using Readarr.Core.Datastore;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv
{
    // Stub class for TV compatibility - to be removed
    public class Season : ModelBase
    {
        public int SeriesId { get; set; }
        public int SeasonNumber { get; set; }
        public bool Monitored { get; set; }
    }
}