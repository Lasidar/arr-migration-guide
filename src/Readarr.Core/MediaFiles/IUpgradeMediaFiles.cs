using System.Collections.Generic;
using Readarr.Core.MediaFiles.BookImport;
using Readarr.Core.MediaFiles.EpisodeImport;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles
{
    public interface IUpgradeMediaFiles
    {
        BookFileUpgradeResult UpgradeBookFile(BookFile bookFile, LocalBook localBook, bool copyOnly = false);
        EpisodeFileUpgradeResult UpgradeEpisodeFile(EpisodeFile episodeFile, LocalEpisode localEpisode, bool copyOnly = false);
    }

    public class BookFileUpgradeResult
    {
        public BookFile BookFile { get; set; }
        public List<BookFile> OldFiles { get; set; }

        public BookFileUpgradeResult()
        {
            OldFiles = new List<BookFile>();
        }
    }
    
    public class EpisodeFileUpgradeResult
    {
        public EpisodeFile EpisodeFile { get; set; }
        public List<EpisodeFile> OldFiles { get; set; }

        public EpisodeFileUpgradeResult()
        {
            OldFiles = new List<EpisodeFile>();
        }
    }
}