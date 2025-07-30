using System.Collections.Generic;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookGrabPayload : WebhookPayload
    {
        public WebhookAuthor Author { get; set; }
        public List<WebhookBook> Books { get; set; }
        public WebhookRelease Release { get; set; }
        public string DownloadClient { get; set; }
        public string DownloadClientType { get; set; }
        public string DownloadId { get; set; }
        public WebhookCustomFormatInfo CustomFormatInfo { get; set; }
    }
}