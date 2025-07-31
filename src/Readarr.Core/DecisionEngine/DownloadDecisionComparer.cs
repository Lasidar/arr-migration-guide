using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.Indexers;
using Readarr.Core.Parser.Model;
using Readarr.Core.Profiles.Delay;
using Readarr.Core.Qualities;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine
{
    public class DownloadDecisionComparer : IComparer<DownloadDecision>
    {
        private readonly IConfigService _configService;
        private readonly IDelayProfileService _delayProfileService;

        public delegate int CompareDelegate(DownloadDecision x, DownloadDecision y);
        public delegate int CompareDelegate<TSubject, TValue>(DownloadDecision x, DownloadDecision y);

        public DownloadDecisionComparer(IConfigService configService, IDelayProfileService delayProfileService)
        {
            _configService = configService;
            _delayProfileService = delayProfileService;
        }

        public int Compare(DownloadDecision x, DownloadDecision y)
        {
            var comparers = new List<CompareDelegate>
            {
                CompareQuality,
                CompareCustomFormatScore,
                CompareProtocol,
                CompareEpisodeCount,
                CompareEpisodeNumber,
                CompareIndexerPriority,
                ComparePeersIfTorrent,
                CompareAgeIfUsenet,
                CompareSize
            };

            return comparers.Select(comparer => comparer(x, y)).FirstOrDefault(result => result != 0);
        }

        private int CompareBy<TSubject, TValue>(TSubject left, TSubject right, Func<TSubject, TValue> funcValue)
            where TValue : IComparable<TValue>
        {
            var leftValue = funcValue(left);
            var rightValue = funcValue(right);

            return leftValue.CompareTo(rightValue);
        }

        private int CompareByReverse<TSubject, TValue>(TSubject left, TSubject right, Func<TSubject, TValue> funcValue)
            where TValue : IComparable<TValue>
        {
            return CompareBy(left, right, funcValue) * -1;
        }

        private int CompareAll(params int[] comparers)
        {
            return comparers.Select(comparer => comparer).FirstOrDefault(result => result != 0);
        }

        private int CompareIndexerPriority(DownloadDecision x, DownloadDecision y)
        {
            return CompareByReverse(x.RemoteBook.Release, y.RemoteBook.Release, release => release.IndexerPriority);
        }

        private int CompareQuality(DownloadDecision x, DownloadDecision y)
        {
            if (_configService.DownloadPropersAndRepacks == ProperDownloadTypes.DoNotPrefer)
            {
                return CompareBy(x.RemoteBook, y.RemoteBook, remoteBook => remoteBook.Author.QualityProfile.Value.GetIndex(remoteBook.ParsedBookInfo.Quality.Quality));
            }

            return CompareAll(
                CompareBy(x.RemoteBook, y.RemoteBook, remoteBook => remoteBook.Author.QualityProfile.Value.GetIndex(remoteBook.ParsedBookInfo.Quality.Quality)),
                CompareBy(x.RemoteBook, y.RemoteBook, remoteBook => remoteBook.ParsedBookInfo.Quality.Revision));
        }

        private int CompareCustomFormatScore(DownloadDecision x, DownloadDecision y)
        {
            return CompareBy(x.RemoteBook, y.RemoteBook, remoteBook => remoteBook.CustomFormatScore);
        }

        private int CompareProtocol(DownloadDecision x, DownloadDecision y)
        {
            var result = CompareBy(x.RemoteBook, y.RemoteBook, remoteBook =>
            {
                var delayProfile = _delayProfileService.BestForTags(remoteBook.Author.Tags);
                var downloadProtocol = remoteBook.Release.DownloadProtocol;
                return downloadProtocol == delayProfile.PreferredProtocol;
            });

            return result;
        }

        private int CompareEpisodeCount(DownloadDecision x, DownloadDecision y)
        {
            // TODO: Implement book count comparison
            // For now, just compare by book count if available
            return CompareBy(x.RemoteBook, y.RemoteBook, remoteBook => remoteBook.Books?.Count ?? 1);
        }

        private int CompareEpisodeNumber(DownloadDecision x, DownloadDecision y)
        {
            // TODO: Implement book number comparison
            // For now, return 0 as books don't have episode numbers
            return 0;
        }

        private int ComparePeersIfTorrent(DownloadDecision x, DownloadDecision y)
        {
            // Different protocols should get caught when checking the preferred protocol,
            // since we're dealing with the same series in our comparisons
            if (x.RemoteBook.Release.DownloadProtocol != DownloadProtocol.Torrent ||
                y.RemoteBook.Release.DownloadProtocol != DownloadProtocol.Torrent)
            {
                return 0;
            }

            return CompareAll(
                CompareBy(x.RemoteBook, y.RemoteBook, remoteBook =>
                {
                    var seeders = TorrentInfo.GetSeeders(remoteBook.Release);

                    return seeders.HasValue && seeders.Value > 0 ? Math.Round(Math.Log10(seeders.Value)) : 0;
                }),
                CompareBy(x.RemoteBook, y.RemoteBook, remoteBook =>
                {
                    var peers = TorrentInfo.GetPeers(remoteBook.Release);

                    return peers.HasValue && peers.Value > 0 ? Math.Round(Math.Log10(peers.Value)) : 0;
                }));
        }

        private int CompareAgeIfUsenet(DownloadDecision x, DownloadDecision y)
        {
            if (x.RemoteBook.Release.DownloadProtocol != DownloadProtocol.Usenet ||
                y.RemoteBook.Release.DownloadProtocol != DownloadProtocol.Usenet)
            {
                return 0;
            }

            return CompareBy(x.RemoteBook, y.RemoteBook, remoteBook =>
            {
                var ageHours = remoteBook.Release.AgeHours;
                var age = remoteBook.Release.Age;

                if (ageHours < 1)
                {
                    return 1000;
                }

                if (ageHours <= 24)
                {
                    return 100;
                }

                if (age <= 7)
                {
                    return 10;
                }

                return Math.Round(Math.Log10(age)) * -1;
            });
        }

        private int CompareSize(DownloadDecision x, DownloadDecision y)
        {
            var sizeCompare =  CompareBy(x.RemoteBook, y.RemoteBook, remoteBook =>
            {
                var qualityProfile = remoteBook.Author.QualityProfile.Value;
                var qualityIndex = qualityProfile.GetIndex(remoteBook.ParsedBookInfo.Quality.Quality, true);
                var qualityOrGroup = qualityProfile.Items[qualityIndex.Index];
                var item = qualityOrGroup.Quality == null ? qualityOrGroup.Items[qualityIndex.GroupIndex] : qualityOrGroup;
                var preferredSize = item.PreferredSize;

                // If no value for preferred it means unlimited so fallback to sort largest is best
                if (preferredSize.HasValue)
                {
                    // TODO: Calculate preferred size based on book characteristics
                    // For now, just use the raw size comparison
                    return Math.Abs((remoteBook.Release.Size - preferredSize.Value.Megabytes()).Round(200.Megabytes())) * (-1);
                }
                else
                {
                    return remoteBook.Release.Size.Round(200.Megabytes());
                }
            });

            return sizeCompare;
        }
    }
}
