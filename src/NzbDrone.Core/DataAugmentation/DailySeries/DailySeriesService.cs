using System;
using System.Collections.Generic;
using System.Linq;
using NzbDrone.Common.Cache;

namespace NzbDrone.Core.DataAugmentation.DailySeries
{
    public interface IDailyAuthorService
    {
        bool IsDailySeries(int tvdbid);
    }

    public class DailyAuthorService : IDailyAuthorService
    {
        private readonly IDailySeriesDataProxy _proxy;
        private readonly ICached<List<int>> _cache;

        public DailyAuthorService(IDailySeriesDataProxy proxy, ICacheManager cacheManager)
        {
            _proxy = proxy;
            _cache = cacheManager.GetCache<List<int>>(GetType());
        }

        public bool IsDailySeries(int tvdbid)
        {
            var dailySeries = _cache.Get("all", () => _proxy.GetDailyAuthorIds().ToList(), TimeSpan.FromHours(1));
            return dailySeries.Any(i => i == tvdbid);
        }
    }
}
