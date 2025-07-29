namespace NzbDrone.Core.MediaFiles
{
    public class DeletedEditionFile
    {
        public string RecycleBinPath { get; set; }
        public EditionFile EditionFile { get; set; }

        public DeletedEditionFile(EditionFile episodeFile, string recycleBinPath)
        {
            EditionFile = episodeFile;
            RecycleBinPath = recycleBinPath;
        }
    }
}
