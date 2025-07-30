using Readarr.Core.Download.TrackedDownloads;

namespace Readarr.Core.MediaFiles.BookImport.Manual
{
    public class ManuallyImportedFile
    {
        public TrackedDownload TrackedDownload { get; set; }
        public ImportResult ImportResult { get; set; }
    }
}
