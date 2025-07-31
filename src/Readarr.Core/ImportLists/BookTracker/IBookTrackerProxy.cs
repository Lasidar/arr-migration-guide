using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Readarr.Core.ImportLists.BookTracker
{
    public interface IBookTrackerProxy
    {
        List<BookTrackerBook> GetUserList(BookTrackerSettings settings);
        ValidationFailure Test(BookTrackerSettings settings);
    }

    public class BookTrackerBook
    {
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public string GoodreadsId { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Edition { get; set; }
    }
}