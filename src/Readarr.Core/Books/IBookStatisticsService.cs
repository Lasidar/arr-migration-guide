using System.Collections.Generic;

namespace Readarr.Core.Books
{
    public interface IBookStatisticsService
    {
        List<BookStatistics> BookStatistics();
        BookStatistics GetBookStatistics(int bookId);
    }

    public class BookStatistics
    {
        public int BookId { get; set; }
        public int BookFileCount { get; set; }
        public int EditionCount { get; set; }
        public int TotalEditionCount { get; set; }
        public long SizeOnDisk { get; set; }
    }
}