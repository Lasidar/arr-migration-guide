using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Readarr.Core.ImportLists.Goodreads
{
    public interface IGoodreadsProxy
    {
        List<GoodreadsBook> GetListBooks(string listId, string apiKey);
        ValidationFailure Test(GoodreadsListSettings settings);
    }

    public class GoodreadsBook
    {
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string GoodreadsId { get; set; }
        public string Isbn { get; set; }
        public int? PublicationYear { get; set; }
        public string Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }
    }
}