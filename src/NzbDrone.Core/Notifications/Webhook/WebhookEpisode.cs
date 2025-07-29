using System;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Notifications.Webhook
{
    public class WebhookEpisode
    {
        public WebhookEpisode()
        {
        }

        public WebhookEpisode(Episode episode)
        {
            Id = episode.Id;
            BookNumber = episode.BookNumber;
            EditionNumber = episode.EditionNumber;
            Title = episode.Title;
            Overview = episode.Overview;
            AirDate = episode.AirDate;
            AirDateUtc = episode.AirDateUtc;
            AuthorId = episode.AuthorId;
            TvdbId = episode.TvdbId;
        }

        public int Id { get; set; }
        public int EditionNumber { get; set; }
        public int BookNumber { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string AirDate { get; set; }
        public DateTime? AirDateUtc { get; set; }
        public int AuthorId { get; set; }
        public int TvdbId { get; set; }
    }
}
