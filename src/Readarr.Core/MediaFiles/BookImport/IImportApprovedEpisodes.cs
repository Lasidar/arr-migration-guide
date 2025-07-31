using System.Collections.Generic;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.BookImport
{
    // Stub interface for TV compatibility - to be removed
    public interface IImportApprovedEpisodes
    {
        List<ImportResult> Import(List<ImportDecision<LocalEpisode>> decisions, bool newDownload, DownloadClientItem downloadClientItem = null, ImportMode importMode = ImportMode.Auto);
    }
}