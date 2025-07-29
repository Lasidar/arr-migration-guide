using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Common.Cache;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.IndexerSearch;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books.Events;

namespace NzbDrone.Core.Books
{
    public interface IEpisodeRefreshedService
    {
        void Search(int seriesId);
    }

    public class EditionRefreshedService : IEpisodeRefreshedService, IHandle<EpisodeInfoRefreshedEvent>
    {
        private readonly IManageCommandQueue _commandQueueManager;
        private readonly IEditionService _episodeService;
        private readonly Logger _logger;
        private readonly ICached<List<int>> _searchCache;

        public EpisodeRefreshedService(ICacheManager cacheManager,
                                   IManageCommandQueue commandQueueManager,
                                   IEditionService episodeService,
                                   Logger logger)
        {
            _commandQueueManager = commandQueueManager;
            _episodeService = episodeService;
            _logger = logger;
            _searchCache = cacheManager.GetCache<List<int>>(GetType());
        }

        public void Search(int seriesId)
        {
            var previouslyAired = _searchCache.Find(seriesId.ToString());

            if (previouslyAired != null && previouslyAired.Any())
            {
                var missing = previouslyAired.Select(e => _episodeService.GetEpisode(e)).Where(e => !e.HasFile).ToList();

                if (missing.Any())
                {
                    _commandQueueManager.Push(new EpisodeSearchCommand(missing.Select(e => e.Id).ToList()));
                }
            }

            _searchCache.Remove(seriesId.ToString());
        }

        public void Handle(EpisodeInfoRefreshedEvent message)
        {
            if (message.Series.AddOptions == null)
            {
                var toSearch = new List<int>();

                if (!message.Series.Monitored)
                {
                    _logger.Debug("Series is not monitored");
                    return;
                }

                var previouslyAired = message.Added.Where(a =>
                        a.AirDateUtc.HasValue &&
                        a.AirDateUtc.Value.Between(DateTime.UtcNow.AddDays(-14), DateTime.UtcNow.AddDays(1)) &&
                        a.Monitored)
                    .Select(e => e.Id)
                    .ToList();

                if (previouslyAired.Empty())
                {
                    _logger.Debug("Newly added episodes all air in the future");
                }

                toSearch.AddRange(previouslyAired);

                var absoluteEditionNumberAdded = message.Updated.Where(a =>
                        a.AbsoluteEditionNumberAdded &&
                        a.AirDateUtc.HasValue &&
                        a.AirDateUtc.Value.Between(DateTime.UtcNow.AddDays(-14), DateTime.UtcNow.AddDays(1)) &&
                        a.Monitored)
                    .Select(e => e.Id)
                    .ToList();

                if (absoluteEditionNumberAdded.Empty())
                {
                    _logger.Debug("No updated episodes recently aired and had absolute episode number added");
                }

                toSearch.AddRange(absoluteEditionNumberAdded);

                if (toSearch.Any())
                {
                    _searchCache.Set(message.Series.Id.ToString(), toSearch.Distinct().ToList());
                }
            }
        }
    }
}
