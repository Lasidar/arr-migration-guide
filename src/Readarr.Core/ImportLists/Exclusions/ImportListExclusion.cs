using Readarr.Core.Datastore;

namespace Readarr.Core.ImportLists.Exclusions
{
    public class ImportListExclusion : ModelBase
    {
        public int TvdbId { get; set; }
        public string Title { get; set; }
    }
}
