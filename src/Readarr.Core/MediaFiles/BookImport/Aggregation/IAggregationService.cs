using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.BookImport.Aggregation
{
    // Stub interface for TV compatibility - to be removed
    public interface IAggregationService
    {
        LocalEpisode Augment(LocalEpisode localEpisode, bool otherFiles);
    }
}