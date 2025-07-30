using System.Collections.Generic;
using Readarr.Core.Download;
using Readarr.Core.MediaFiles.BookImport.Manual;

namespace Readarr.Core.MediaFiles.BookImport
{
    public interface IImportApprovedBooks
    {
        List<ImportResult> Import(List<ImportDecision> decisions, bool newDownload, DownloadClientItem downloadClientItem = null, ImportMode importMode = ImportMode.Auto);
    }
}