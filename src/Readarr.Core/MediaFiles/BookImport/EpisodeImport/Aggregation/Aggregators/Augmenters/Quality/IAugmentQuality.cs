using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.BookImport.Aggregation.Aggregators.Augmenters.Quality
{
    public interface IAugmentQuality
    {
        int Order { get; }
        string Name { get; }
        AugmentQualityResult AugmentQuality(LocalEpisode localEpisode, DownloadClientItem downloadClientItem);
    }
}
