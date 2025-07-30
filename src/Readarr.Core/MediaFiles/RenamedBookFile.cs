namespace Readarr.Core.MediaFiles
{
    public class RenamedBookFile
    {
        public BookFile BookFile { get; set; }
        public string PreviousPath { get; set; }
        public string PreviousRelativePath { get; set; }
    }
}