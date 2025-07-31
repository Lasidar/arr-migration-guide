namespace Readarr.Core.Notifications.Webhook
{
    // Stub class for TV compatibility - to be removed
    public class WebhookEpisodeDeletePayload : WebhookPayload
    {
        public WebhookSeries Series { get; set; }
        public WebhookEpisode[] Episodes { get; set; }
        public WebhookEpisodeFile EpisodeFile { get; set; }
        public string DeleteReason { get; set; }
    }
}