using System.Collections.Generic;
using System.Linq;
using Readarr.Core.AuthorStats;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Books
{
    public class BookStatisticsService : IBookStatisticsService
    {
        private readonly IBookStatisticsRepository _bookStatisticsRepository;

        public BookStatisticsService(IBookStatisticsRepository bookStatisticsRepository)
        {
            _bookStatisticsRepository = bookStatisticsRepository;
        }

        public List<BookStatistics> BookStatistics()
        {
            return _bookStatisticsRepository.BookStatistics();
        }

        public BookStatistics GetBookStatistics(int bookId)
        {
            var statistics = _bookStatisticsRepository.BookStatistics(bookId);

            if (statistics != null)
            {
                return statistics;
            }

            return new BookStatistics
            {
                BookId = bookId,
                EditionCount = 0,
                TotalEditionCount = 0,
                BookFileCount = 0,
                SizeOnDisk = 0
            };
        }

        public List<BookStatistics> GetBookStatisticsByAuthor(int authorId)
        {
            return _bookStatisticsRepository.BookStatisticsByAuthor(authorId);
        }
    }

    public interface IBookStatisticsRepository
    {
        List<BookStatistics> BookStatistics();
        BookStatistics BookStatistics(int bookId);
        List<BookStatistics> BookStatisticsByAuthor(int authorId);
    }
}