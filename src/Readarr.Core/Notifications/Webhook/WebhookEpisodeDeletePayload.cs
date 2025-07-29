using System.Collections.Generic;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookEpisodeDeletePayload : WebhookPayload
    {
        public WebhookSeries Series { get; set; }
        public List<WebhookEpisode> Episodes { get; set; }
        public WebhookEpisodeFile EpisodeFile { get; set; }
        public DeleteMediaFileReason DeleteReason { get; set; }
    }
}
