namespace NzbDrone.Core.MediaFiles
{
    public class RenamedEditionFile
    {
        public EditionFile EditionFile { get; set; }
        public string PreviousPath { get; set; }
        public string PreviousRelativePath { get; set; }
    }
}
