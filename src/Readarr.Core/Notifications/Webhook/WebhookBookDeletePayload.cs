using System.Collections.Generic;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookDeletePayload : WebhookPayload
    {
        public WebhookAuthor Author { get; set; }
        public List<WebhookBook> Books { get; set; }
        public bool DeleteFiles { get; set; }
    }
}