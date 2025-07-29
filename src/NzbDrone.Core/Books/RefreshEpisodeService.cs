using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books.Events;

namespace NzbDrone.Core.Books
{
    public interface IRefreshEditionService
    {
        void RefreshEpisodeInfo(Series series, IEnumerable<Episode> remoteEpisodes);
    }

    public class RefreshEditionService : IRefreshEditionService
    {
        private readonly IEditionService _episodeService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public RefreshEditionService(IEditionService episodeService, IEventAggregator eventAggregator, Logger logger)
        {
            _episodeService = episodeService;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public void RefreshEpisodeInfo(Series series, IEnumerable<Episode> remoteEpisodes)
        {
            _logger.Info("Starting episode info refresh for: {0}", series);
            var successCount = 0;
            var failCount = 0;

            var existingEpisodes = _episodeService.GetEpisodeBySeries(series.Id);
            var seasons = series.Seasons;
            var hasExisting = existingEpisodes.Any();

            var updateList = new List<Episode>();
            var newList = new List<Episode>();
            var dupeFreeRemoteEpisodes = remoteEpisodes.DistinctBy(m => new { m.BookNumber, m.EditionNumber }).ToList();

            if (series.SeriesType == SeriesTypes.Anime)
            {
                dupeFreeRemoteEpisodes = MapAbsoluteEditionNumbers(dupeFreeRemoteEpisodes);
            }

            var orderedEpisodes = OrderEpisodes(series, dupeFreeRemoteEpisodes).ToList();
            var episodesPerSeason = orderedEpisodes.GroupBy(s => s.BookNumber).ToDictionary(g => g.Key, g => g.Count());
            var latestSeason = seasons.MaxBy(s => s.BookNumber);

            foreach (var episode in orderedEpisodes)
            {
                try
                {
                    var episodeToUpdate = existingEpisodes.FirstOrDefault(e => e.BookNumber == episode.BookNumber && e.EditionNumber == episode.EditionNumber);

                    if (episodeToUpdate != null)
                    {
                        existingEpisodes.Remove(episodeToUpdate);
                        updateList.Add(episodeToUpdate);

                        // Anime series with newly added absolute episode number
                        if (series.SeriesType == SeriesTypes.Anime &&
                            !episodeToUpdate.AbsoluteEditionNumber.HasValue &&
                            episode.AbsoluteEditionNumber.HasValue)
                        {
                            episodeToUpdate.AbsoluteEditionNumberAdded = true;
                        }
                    }
                    else
                    {
                        episodeToUpdate = new Episode();
                        episodeToUpdate.Monitored = GetMonitoredStatus(episode, seasons, series);
                        newList.Add(episodeToUpdate);
                    }

                    episodeToUpdate.AuthorId = series.Id;
                    episodeToUpdate.TvdbId = episode.TvdbId;
                    episodeToUpdate.EditionNumber = episode.EditionNumber;
                    episodeToUpdate.BookNumber = episode.BookNumber;
                    episodeToUpdate.AbsoluteEditionNumber = episode.AbsoluteEditionNumber;
                    episodeToUpdate.AiredAfterBookNumber = episode.AiredAfterBookNumber;
                    episodeToUpdate.AiredBeforeBookNumber = episode.AiredBeforeBookNumber;
                    episodeToUpdate.AiredBeforeEditionNumber = episode.AiredBeforeEditionNumber;
                    episodeToUpdate.Title = episode.Title ?? "TBA";
                    episodeToUpdate.Overview = episode.Overview;
                    episodeToUpdate.AirDate = episode.AirDate;
                    episodeToUpdate.AirDateUtc = episode.AirDateUtc;
                    episodeToUpdate.Runtime = episode.Runtime;
                    episodeToUpdate.FinaleType = episode.FinaleType;
                    episodeToUpdate.Ratings = episode.Ratings;
                    episodeToUpdate.Images = episode.Images;

                    // TheTVDB has a severe lack of season/series finales, this helps smooth out that limitation so they can be displayed in the UI
                    if (series.Status == SeriesStatusType.Ended &&
                        episodeToUpdate.FinaleType == null &&
                        episodeToUpdate.BookNumber > 0 &&
                        episodeToUpdate.BookNumber == latestSeason.BookNumber &&
                        episodeToUpdate.EditionNumber > 1 &&
                        episodeToUpdate.EditionNumber == episodesPerSeason[episodeToUpdate.BookNumber] &&
                        episodeToUpdate.AirDateUtc.HasValue &&
                        episodeToUpdate.AirDateUtc.Value.After(DateTime.UtcNow.AddDays(-14)) &&
                        orderedEpisodes.None(e => e.BookNumber == latestSeason.BookNumber && e.FinaleType != null))
                    {
                        episodeToUpdate.FinaleType = "series";
                    }

                    successCount++;
                }
                catch (Exception e)
                {
                    _logger.Fatal(e, "An error has occurred while updating episode info for series {0}. {1}", series, episode);
                    failCount++;
                }
            }

            UnmonitorReaddedEpisodes(series, newList, hasExisting);

            var allEpisodes = new List<Episode>();
            allEpisodes.AddRange(newList);
            allEpisodes.AddRange(updateList);

            AdjustMultiEpisodeAirTime(series, allEpisodes);
            AdjustDirectToDvdAirDate(series, allEpisodes);

            _episodeService.DeleteMany(existingEpisodes);
            _episodeService.UpdateMany(updateList);
            _episodeService.InsertMany(newList);

            _eventAggregator.PublishEvent(new EpisodeInfoRefreshedEvent(series, newList, updateList, existingEpisodes));

            if (failCount != 0)
            {
                _logger.Info("Finished episode refresh for series: {0}. Successful: {1} - Failed: {2} ",
                    series.Title,
                    successCount,
                    failCount);
            }
            else
            {
                _logger.Info("Finished episode refresh for series: {0}.", series);
            }
        }

        private bool GetMonitoredStatus(Episode episode, IEnumerable<Season> seasons, Series series)
        {
            if (episode.EditionNumber == 0 && episode.BookNumber != 1)
            {
                return false;
            }

            var season = seasons.SingleOrDefault(c => c.BookNumber == episode.BookNumber);
            return season == null || season.Monitored;
        }

        private void UnmonitorReaddedEpisodes(Series series, List<Episode> episodes, bool hasExisting)
        {
            if (series.AddOptions != null)
            {
                return;
            }

            var threshold = DateTime.UtcNow.AddDays(-14);

            var oldEpisodes = episodes.Where(e => e.AirDateUtc.HasValue && e.AirDateUtc.Value.Before(threshold)).ToList();

            if (oldEpisodes.Any())
            {
                if (hasExisting)
                {
                    _logger.Warn("Show {0} ({1}) had {2} old episodes appear, please check monitored status.", series.TvdbId, series.Title, oldEpisodes.Count);
                }
                else
                {
                    threshold = DateTime.UtcNow.AddDays(-1);

                    foreach (var episode in episodes)
                    {
                        if (episode.AirDateUtc.HasValue && episode.AirDateUtc.Value.Before(threshold))
                        {
                            episode.Monitored = false;
                        }
                    }

                    _logger.Warn("Show {0} ({1}) had {2} old episodes appear, unmonitored aired episodes to prevent unexpected downloads.", series.TvdbId, series.Title, oldEpisodes.Count);
                }
            }
        }

        private void AdjustMultiEpisodeAirTime(Series series, IEnumerable<Episode> allEpisodes)
        {
            var groups = allEpisodes.Where(c => c.AirDateUtc.HasValue)
                                    .GroupBy(e => new { e.BookNumber, e.AirDate })
                                    .Where(g => g.Count() > 1)
                                    .ToList();

            foreach (var group in groups)
            {
                if (group.Key.BookNumber != 0 && group.Count() > 3)
                {
                    _logger.Debug("Not adjusting multi-episode air times for series {0} season {1} since more than 3 episodes 'aired' on the same day", series.Title, group.Key.BookNumber);
                    continue;
                }

                var episodeCount = 0;

                foreach (var episode in group.OrderBy(e => e.BookNumber).ThenBy(e => e.EditionNumber))
                {
                    episode.AirDateUtc = episode.AirDateUtc.Value.AddMinutes(series.Runtime * episodeCount);
                    episodeCount++;
                }
            }
        }

        private void AdjustDirectToDvdAirDate(Series series, IList<Episode> allEpisodes)
        {
            if (series.Status == SeriesStatusType.Ended && allEpisodes.All(v => !v.AirDateUtc.HasValue) && series.FirstAired.HasValue)
            {
                foreach (var episode in allEpisodes)
                {
                    episode.AirDateUtc = series.FirstAired;
                    episode.AirDate = series.FirstAired.Value.ToString("yyyy-MM-dd");
                }
            }
        }

        private List<Episode> MapAbsoluteEditionNumbers(List<Episode> remoteEpisodes)
        {
            // Return all episodes with no abs number, but distinct for those with abs number
            return remoteEpisodes.Where(e => e.AbsoluteEditionNumber.HasValue)
                                 .OrderByDescending(e => e.BookNumber)
                                 .DistinctBy(e => e.AbsoluteEditionNumber.Value)
                                 .Concat(remoteEpisodes.Where(e => !e.AbsoluteEditionNumber.HasValue))
                                 .ToList();
        }

        private IEnumerable<Episode> OrderEpisodes(Series series, List<Episode> episodes)
        {
            if (series.SeriesType == SeriesTypes.Anime)
            {
                var withAbs = episodes.Where(e => e.AbsoluteEditionNumber.HasValue)
                                      .OrderBy(e => e.AbsoluteEditionNumber);

                var withoutAbs = episodes.Where(e => !e.AbsoluteEditionNumber.HasValue)
                                         .OrderBy(e => e.BookNumber)
                                         .ThenBy(e => e.EditionNumber);

                return withAbs.Concat(withoutAbs);
            }

            return episodes.OrderBy(e => e.BookNumber).ThenBy(e => e.EditionNumber);
        }
    }
}
