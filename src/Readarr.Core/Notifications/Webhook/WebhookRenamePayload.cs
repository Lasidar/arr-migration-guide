using System.Collections.Generic;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookRenamePayload : WebhookPayload
    {
        // Book properties
        public WebhookAuthor Author { get; set; }
        public List<WebhookRenamedBookFile> RenamedBookFiles { get; set; }
        
        // TV properties (to be removed)
        public WebhookSeries Series { get; set; }
        public List<WebhookRenamedEpisodeFile> RenamedEpisodeFiles { get; set; }
    }
}
