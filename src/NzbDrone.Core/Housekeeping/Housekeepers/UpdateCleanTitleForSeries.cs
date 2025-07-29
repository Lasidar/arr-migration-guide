using System.Linq;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class UpdateCleanTitleForSeries : IHousekeepingTask
    {
        private readonly IAuthorRepository _seriesRepository;

        public UpdateCleanTitleForSeries(IAuthorRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public void Clean()
        {
            var series = _seriesRepository.All().ToList();

            series.ForEach(s =>
            {
                var cleanTitle = s.Title.CleanSeriesTitle();
                if (s.CleanTitle != cleanTitle)
                {
                    s.CleanTitle = cleanTitle;
                    _seriesRepository.Update(s);
                }
            });
        }
    }
}
