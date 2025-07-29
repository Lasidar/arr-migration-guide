using Readarr.Core.Parser.Model;

namespace Readarr.Core.Download.Aggregation.Aggregators
{
    public interface IAggregateRemoteEpisode
    {
        RemoteEpisode Aggregate(RemoteEpisode remoteEpisode);
    }
}
