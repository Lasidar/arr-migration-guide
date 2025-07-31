using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Localization;
using Readarr.Core.Books;
using Readarr.Core.Books.Events;

namespace Readarr.Core.HealthCheck.Checks
{
    [CheckOn(typeof(AuthorUpdatedEvent))]
    [CheckOn(typeof(AuthorDeletedEvent))]
    [CheckOn(typeof(SeriesRefreshCompleteEvent))]
    public class RemovedSeriesCheck : HealthCheckBase, ICheckOnCondition<AuthorUpdatedEvent>, ICheckOnCondition<AuthorDeletedEvent>
    {
        private readonly ISeriesService _seriesService;

        public RemovedSeriesCheck(ISeriesService seriesService, ILocalizationService localizationService)
            : base(localizationService)
        {
            _seriesService = seriesService;
        }

        public override HealthCheck Check()
        {
            var deletedSeries = _seriesService.GetAllSeries().Where(v => v.Status == SeriesStatusType.Deleted).ToList();

            if (deletedSeries.Empty())
            {
                return new HealthCheck(GetType());
            }

            var seriesText = deletedSeries.Select(s => $"{s.Title} (tvdbid {s.TvdbId})").Join(", ");

            if (deletedSeries.Count == 1)
            {
                return new HealthCheck(GetType(),
                    HealthCheckResult.Error,
                    _localizationService.GetLocalizedString("RemovedSeriesSingleRemovedHealthCheckMessage", new Dictionary<string, object>
                    {
                        { "series", seriesText }
                    }),
                    "#series-removed-from-thetvdb");
            }

            return new HealthCheck(GetType(),
                HealthCheckResult.Error,
                _localizationService.GetLocalizedString("RemovedSeriesMultipleRemovedHealthCheckMessage", new Dictionary<string, object>
                {
                    { "series", seriesText }
                }),
                "#series-removed-from-thetvdb");
        }

        public bool ShouldCheckOnEvent(AuthorDeletedEvent deletedEvent)
        {
            return deletedEvent.Series.Any(s => s.Status == SeriesStatusType.Deleted);
        }

        public bool ShouldCheckOnEvent(AuthorUpdatedEvent updatedEvent)
        {
            return updatedEvent.Series.Status == SeriesStatusType.Deleted;
        }
    }
}
