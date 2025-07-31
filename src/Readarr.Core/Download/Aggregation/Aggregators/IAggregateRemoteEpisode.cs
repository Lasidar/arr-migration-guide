using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.Download.Aggregation.Aggregators
{
    public interface IAggregateRemoteEpisode
    {
        RemoteEpisode Aggregate(RemoteEpisode remoteEpisode);
    }
}
