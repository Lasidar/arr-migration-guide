using System.Collections.Generic;

namespace NzbDrone.Core.Notifications.Webhook
{
    public class WebhookImportCompletePayload : WebhookPayload
    {
        public WebhookSeries Series { get; set; }
        public List<WebhookEpisode> Episodes { get; set; }
        public List<WebhookEditionFile> EditionFiles { get; set; }
        public string DownloadClient { get; set; }
        public string DownloadClientType { get; set; }
        public string DownloadId { get; set; }
        public WebhookGrabbedRelease Release { get; set; }
        public int FileCount => EditionFiles.Count;
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }
}
