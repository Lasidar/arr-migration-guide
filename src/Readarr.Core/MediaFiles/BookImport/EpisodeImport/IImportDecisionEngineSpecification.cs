using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.BookImport
{
    public interface IImportDecisionEngineSpecification
    {
        ImportSpecDecision IsSatisfiedBy(LocalEpisode localEpisode, DownloadClientItem downloadClientItem);
    }
}
