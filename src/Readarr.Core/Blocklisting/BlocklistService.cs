using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Download;
using Readarr.Core.Indexers;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser.Model;
using Readarr.Core.Books.Events;

namespace Readarr.Core.Blocklisting
{
    public interface IBlocklistService
    {
        // Book methods
        bool Blocklisted(int authorId, ReleaseInfo release);
        bool BlocklistedTorrentHash(int authorId, string hash);
        void Block(RemoteBook remoteBook, string message);
        
        // TV methods (to be removed)
        void Block(RemoteEpisode remoteEpisode, string message);
        
        // Common methods
        PagingSpec<Blocklist> Paged(PagingSpec<Blocklist> pagingSpec);
        void Delete(int id);
        void Delete(List<int> ids);
    }

    public class BlocklistService : IBlocklistService,
                                    IExecute<ClearBlocklistCommand>,
                                    IHandle<DownloadFailedEvent>,
                                    IHandleAsync<AuthorDeletedEvent>
    {
        private readonly IBlocklistRepository _blocklistRepository;

        public BlocklistService(IBlocklistRepository blocklistRepository)
        {
            _blocklistRepository = blocklistRepository;
        }

        public bool Blocklisted(int authorId, ReleaseInfo release)
        {
            if (release.DownloadProtocol == DownloadProtocol.Torrent)
            {
                if (release is not TorrentInfo torrentInfo)
                {
                    return false;
                }

                if (torrentInfo.InfoHash.IsNotNullOrWhiteSpace())
                {
                    var blocklistedByTorrentInfohash = _blocklistRepository.BlocklistedByTorrentInfoHash(authorId, torrentInfo.InfoHash);

                    return blocklistedByTorrentInfohash.Any(b => SameTorrent(b, torrentInfo));
                }

                return _blocklistRepository.BlocklistedByTitle(authorId, release.Title)
                    .Where(b => b.Protocol == DownloadProtocol.Torrent)
                    .Any(b => SameTorrent(b, torrentInfo));
            }

            return _blocklistRepository.BlocklistedByTitle(authorId, release.Title)
                .Where(b => b.Protocol == DownloadProtocol.Usenet)
                .Any(b => SameNzb(b, release));
        }

        public bool BlocklistedTorrentHash(int authorId, string hash)
        {
            return _blocklistRepository.BlocklistedByTorrentInfoHash(authorId, hash).Any(b =>
                b.TorrentInfoHash.Equals(hash, StringComparison.InvariantCultureIgnoreCase));
        }

        public PagingSpec<Blocklist> Paged(PagingSpec<Blocklist> pagingSpec)
        {
            return _blocklistRepository.GetPaged(pagingSpec);
        }

        public void Block(RemoteEpisode remoteEpisode, string message)
        {
            var blocklist = new Blocklist
                            {
                                SeriesId = remoteEpisode.Series.Id,
                                EpisodeIds = remoteEpisode.Episodes.Select(e => e.Id).ToList(),
                                SourceTitle =  remoteEpisode.Release.Title,
                                Quality = remoteEpisode.ParsedEpisodeInfo.Quality,
                                Date = DateTime.UtcNow,
                                PublishedDate = remoteEpisode.Release.PublishDate,
                                Size = remoteEpisode.Release.Size,
                                Indexer = remoteEpisode.Release.Indexer,
                                Protocol = remoteEpisode.Release.DownloadProtocol,
                                Message = message,
                                Languages = remoteEpisode.ParsedEpisodeInfo.Languages
                            };

            if (remoteEpisode.Release is TorrentInfo torrentRelease)
            {
                blocklist.TorrentInfoHash = torrentRelease.InfoHash;
            }

            _blocklistRepository.Insert(blocklist);
        }

        public void Block(RemoteBook remoteBook, string message)
        {
            var blocklist = new Blocklist
            {
                AuthorId = remoteBook.Author.Id,
                BookIds = remoteBook.Books.Select(b => b.Id).ToList(),
                SourceTitle = remoteBook.Release.Title,
                Quality = remoteBook.Quality,
                Date = DateTime.UtcNow,
                PublishedDate = remoteBook.Release.PublishDate,
                Size = remoteBook.Release.Size,
                Indexer = remoteBook.Release.Indexer,
                Protocol = remoteBook.Release.DownloadProtocol,
                Message = message,
                Languages = remoteBook.Languages
            };

            if (remoteBook.Release is TorrentInfo torrentRelease)
            {
                blocklist.TorrentInfoHash = torrentRelease.InfoHash;
            }

            _blocklistRepository.Insert(blocklist);
        }

        public void Delete(int id)
        {
            _blocklistRepository.Delete(id);
        }

        public void Delete(List<int> ids)
        {
            _blocklistRepository.DeleteMany(ids);
        }

        private bool SameNzb(Blocklist item, ReleaseInfo release)
        {
            if (item.PublishedDate == release.PublishDate)
            {
                return true;
            }

            if (!HasSameIndexer(item, release.Indexer) &&
                HasSamePublishedDate(item, release.PublishDate) &&
                HasSameSize(item, release.Size))
            {
                return true;
            }

            return false;
        }

        private bool SameTorrent(Blocklist item, TorrentInfo release)
        {
            if (release.InfoHash.IsNotNullOrWhiteSpace())
            {
                return release.InfoHash.Equals(item.TorrentInfoHash, StringComparison.InvariantCultureIgnoreCase);
            }

            return HasSameIndexer(item, release.Indexer);
        }

        private bool HasSameIndexer(Blocklist item, string indexer)
        {
            if (item.Indexer.IsNullOrWhiteSpace())
            {
                return true;
            }

            return item.Indexer.Equals(indexer, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool HasSamePublishedDate(Blocklist item, DateTime publishedDate)
        {
            if (!item.PublishedDate.HasValue)
            {
                return true;
            }

            return item.PublishedDate.Value.AddMinutes(-2) <= publishedDate &&
                   item.PublishedDate.Value.AddMinutes(2) >= publishedDate;
        }

        private bool HasSameSize(Blocklist item, long size)
        {
            if (!item.Size.HasValue)
            {
                return true;
            }

            var difference = Math.Abs(item.Size.Value - size);

            return difference <= 2.Megabytes();
        }

        public void Execute(ClearBlocklistCommand message)
        {
            _blocklistRepository.Purge();
        }

        public void Handle(DownloadFailedEvent message)
        {
            var blocklist = new Blocklist
            {
                SeriesId = message.SeriesId,
                EpisodeIds = message.EpisodeIds,
                SourceTitle = message.SourceTitle,
                Quality = message.Quality,
                Date = DateTime.UtcNow,
                PublishedDate = DateTime.Parse(message.Data.GetValueOrDefault("publishedDate")),
                Size = long.Parse(message.Data.GetValueOrDefault("size", "0")),
                Indexer = message.Data.GetValueOrDefault("indexer"),
                Protocol = (DownloadProtocol)Convert.ToInt32(message.Data.GetValueOrDefault("protocol")),
                Message = message.Message,
                Languages = message.Languages,
                TorrentInfoHash = message.TrackedDownload?.Protocol == DownloadProtocol.Torrent
                    ? message.TrackedDownload.DownloadItem.DownloadId
                    : message.Data.GetValueOrDefault("torrentInfoHash", null)
            };

            if (Enum.TryParse(message.Data.GetValueOrDefault("indexerFlags"), true, out IndexerFlags flags))
            {
                blocklist.IndexerFlags = flags;
            }

            if (Enum.TryParse(message.Data.GetValueOrDefault("releaseType"), true, out ReleaseType releaseType))
            {
                blocklist.ReleaseType = releaseType;
            }

            _blocklistRepository.Insert(blocklist);
        }

        public void HandleAsync(AuthorDeletedEvent message)
        {
            _blocklistRepository.DeleteForSeriesIds(message.Series.Select(m => m.Id).ToList());
        }
    }
}
