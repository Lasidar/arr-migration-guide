using System.Linq;
using Readarr.Core.Parser;
using Readarr.Core.Tv;

namespace Readarr.Core.Housekeeping.Housekeepers
{
    public class UpdateCleanTitleForSeries : IHousekeepingTask
    {
        private readonly ISeriesRepository _seriesRepository;

        public UpdateCleanTitleForSeries(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public void Clean()
        {
            var series = _seriesRepository.All().ToList();

            series.ForEach(s =>
            {
                var cleanTitle = Parser.Parser.CleanSeriesTitle(s.Title);
                if (s.CleanTitle != cleanTitle)
                {
                    s.CleanTitle = cleanTitle;
                    _seriesRepository.Update(s);
                }
            });
        }
    }
}
