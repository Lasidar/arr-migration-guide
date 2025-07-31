using System.Collections.Generic;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.BookImport
{
    // Stub interface for TV compatibility - to be removed
    public interface IMakeImportDecision
    {
        List<ImportDecision<LocalEpisode>> GetImportDecisions(List<string> videoFiles, Tv.Series series);
        List<ImportDecision<LocalEpisode>> GetImportDecisions(List<string> videoFiles, Tv.Series series, DownloadClientItem downloadClientItem, ParsedEpisodeInfo folderInfo, bool sceneSource);
    }
}