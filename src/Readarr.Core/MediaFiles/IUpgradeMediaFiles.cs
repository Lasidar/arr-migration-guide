using System.Collections.Generic;
using Readarr.Core.MediaFiles.BookImport;

namespace Readarr.Core.MediaFiles
{
    public interface IUpgradeMediaFiles
    {
        BookFileUpgradeResult UpgradeBookFile(BookFile bookFile, LocalBook localBook, bool copyOnly = false);
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
}