using System;
using NLog;
using Readarr.Core.Configuration;

namespace Readarr.Core.Books
{
    public interface ICheckIfBookShouldBeRefreshed
    {
        bool ShouldRefresh(int? bookId);
    }

    public class CheckIfBookShouldBeRefreshed : ICheckIfBookShouldBeRefreshed
    {
        private readonly IBookService _bookService;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public CheckIfBookShouldBeRefreshed(IBookService bookService,
                                           IConfigService configService,
                                           Logger logger)
        {
            _bookService = bookService;
            _configService = configService;
            _logger = logger;
        }

        public bool ShouldRefresh(int? bookId)
        {
            if (bookId.HasValue)
            {
                var book = _bookService.GetBook(bookId.Value);
                return ShouldRefresh(book);
            }

            return true;
        }

        private bool ShouldRefresh(Book book)
        {
            // Check if book needs refresh based on last refresh time
            var refreshInterval = _configService.MetadataRefreshInterval;
            
            if (book.LastInfoSync == null)
            {
                _logger.Trace($"Book {book.Title} has never been refreshed");
                return true;
            }

            var timeSinceLastRefresh = DateTime.UtcNow - book.LastInfoSync.Value;
            
            if (timeSinceLastRefresh > TimeSpan.FromDays(refreshInterval))
            {
                _logger.Trace($"Book {book.Title} last refreshed {timeSinceLastRefresh.TotalDays:N1} days ago, should refresh");
                return true;
            }

            _logger.Trace($"Book {book.Title} last refreshed {timeSinceLastRefresh.TotalDays:N1} days ago, skipping");
            return false;
        }
    }
}