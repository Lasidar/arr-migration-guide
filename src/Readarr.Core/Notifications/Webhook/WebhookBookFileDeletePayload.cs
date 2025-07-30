using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookFileDeletePayload : WebhookPayload
    {
        public WebhookAuthor Author { get; set; }
        public WebhookBook Book { get; set; }
        public WebhookBookFile BookFile { get; set; }
        public DeleteMediaFileReason DeleteReason { get; set; }
    }
}