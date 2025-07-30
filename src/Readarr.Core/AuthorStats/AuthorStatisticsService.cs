using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Books;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.AuthorStats
{
    public class AuthorStatisticsService : IAuthorStatisticsService
    {
        private readonly IAuthorStatisticsRepository _authorStatisticsRepository;

        public AuthorStatisticsService(IAuthorStatisticsRepository authorStatisticsRepository)
        {
            _authorStatisticsRepository = authorStatisticsRepository;
        }

        public List<AuthorStatistics> AuthorStatistics()
        {
            var statistics = _authorStatisticsRepository.AuthorStatistics();

            return statistics.GroupBy(s => s.AuthorId).Select(s => MapAuthorStatistics(s.ToList())).ToList();
        }

        public AuthorStatistics GetAuthorStatistics(int authorId)
        {
            var statistics = _authorStatisticsRepository.AuthorStatistics(authorId);

            if (statistics.Any())
            {
                return MapAuthorStatistics(statistics);
            }

            return new AuthorStatistics();
        }

        private AuthorStatistics MapAuthorStatistics(List<BookStatistics> bookStatistics)
        {
            var authorStatistics = new AuthorStatistics
            {
                AuthorId = bookStatistics.First().AuthorId,
                BookFileCount = bookStatistics.Sum(s => s.BookFileCount),
                BookCount = bookStatistics.Sum(s => s.BookCount),
                AvailableBookCount = bookStatistics.Sum(s => s.AvailableBookCount),
                TotalBookCount = bookStatistics.Sum(s => s.TotalBookCount),
                SizeOnDisk = bookStatistics.Sum(s => s.SizeOnDisk),
                BookStatistics = bookStatistics
            };

            return authorStatistics;
        }
    }
}