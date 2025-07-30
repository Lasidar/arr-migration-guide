using System;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Overview { get; set; }

        public WebhookBook() { }

        public WebhookBook(Book book)
        {
            Id = book.Id;
            Title = book.Metadata.Value?.Title;
            AuthorName = book.Author?.Metadata.Value?.Name;
            Isbn = book.Metadata.Value?.Isbn13;
            Asin = book.Metadata.Value?.Asin;
            ReleaseDate = book.Metadata.Value?.ReleaseDate;
            Overview = book.Metadata.Value?.Overview;
        }
    }
}