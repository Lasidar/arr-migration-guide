using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookRenamedBookFile : WebhookBookFile
    {
        public WebhookRenamedBookFile(RenamedBookFile renamedFile) : base(renamedFile.BookFile)
        {
            PreviousPath = renamedFile.PreviousPath;
            PreviousRelativePath = renamedFile.PreviousRelativePath;
        }

        public string PreviousPath { get; set; }
        public string PreviousRelativePath { get; set; }
    }
}