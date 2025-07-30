namespace Readarr.Core.MediaFiles
{
    public class DeletedBookFile
    {
        public BookFile BookFile { get; set; }
        public DeleteMediaFileReason Reason { get; set; }

        public DeletedBookFile(BookFile bookFile, DeleteMediaFileReason reason)
        {
            BookFile = bookFile;
            Reason = reason;
        }
    }
}