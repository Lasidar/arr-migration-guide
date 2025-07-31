using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.Download.Aggregation
{
    // Stub interface for TV compatibility - to be removed
    public interface IRemoteEpisodeAggregationService
    {
        RemoteEpisode Augment(RemoteEpisode remoteEpisode);
    }
}