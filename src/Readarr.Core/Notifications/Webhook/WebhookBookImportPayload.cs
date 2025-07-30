using System.Collections.Generic;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookImportPayload : WebhookPayload
    {
        public WebhookAuthor Author { get; set; }
        public List<WebhookBook> Books { get; set; }
        public WebhookBookFile BookFile { get; set; }
        public bool IsUpgrade { get; set; }
        public string DownloadClient { get; set; }
        public string DownloadId { get; set; }
        public List<WebhookBookFile> DeletedFiles { get; set; }
        public WebhookRelease Release { get; set; }
    }
}