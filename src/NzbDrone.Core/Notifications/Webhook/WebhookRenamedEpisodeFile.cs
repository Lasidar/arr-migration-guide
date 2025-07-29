using NzbDrone.Core.MediaFiles;

namespace NzbDrone.Core.Notifications.Webhook
{
    public class WebhookRenamedEditionFile : WebhookEditionFile
    {
        public WebhookRenamedEditionFile(RenamedEditionFile renamedEpisode)
            : base(renamedEpisode.EditionFile)
        {
            PreviousRelativePath = renamedEpisode.PreviousRelativePath;
            PreviousPath = renamedEpisode.PreviousPath;
        }

        public string PreviousRelativePath { get; set; }
        public string PreviousPath { get; set; }
    }
}
