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
                BookStatistics = bookStatistics,
                AuthorId = bookStatistics.First().AuthorId,
                BookCount = bookStatistics.Count,
                BookFileCount = bookStatistics.Count(s => s.BookFileCount > 0),
                TotalBookCount = bookStatistics.Count,
                SizeOnDisk = bookStatistics.Sum(s => s.SizeOnDisk),
                AvailableBookCount = bookStatistics.Count(s => s.BookFileCount > 0),
                MonitoredBookCount = bookStatistics.Count(s => s.Monitored)
            };

            var bookCounts = bookStatistics.GroupBy(s => s.Monitored).ToDictionary(g => g.Key, g => g.Count());
            authorStatistics.BookCount = bookCounts.GetValueOrDefault(true, 0);
            authorStatistics.UnmonitoredBookCount = bookCounts.GetValueOrDefault(false, 0);

            return authorStatistics;
        }
    }

    public interface IAuthorStatisticsRepository
    {
        List<BookStatistics> AuthorStatistics();
        List<BookStatistics> AuthorStatistics(int authorId);
    }
}