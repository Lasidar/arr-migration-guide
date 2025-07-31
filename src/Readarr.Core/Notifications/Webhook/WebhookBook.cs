using System;
using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBook
    {
        public WebhookBook()
        {
        }

        public WebhookBook(Book book)
        {
            Id = book.Id;
            Title = book.Title;
            AuthorName = book.Author?.Value?.Name;
            GoodreadsId = book.Metadata.Value?.GoodreadsId;
            Isbn = book.Metadata.Value?.Isbn13;
            Asin = book.Metadata.Value?.Asin;
            ReleaseDate = book.ReleaseDate;
            Overview = book.Overview;
            PageCount = book.PageCount;
            Genres = book.Genres;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string GoodreadsId { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Overview { get; set; }
        public int PageCount { get; set; }
        public List<string> Genres { get; set; }
    }
}