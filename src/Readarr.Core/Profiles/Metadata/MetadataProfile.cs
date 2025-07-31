using Readarr.Core.Datastore;
using Readarr.Core.Tv;

namespace Readarr.Core.Profiles.Metadata
{
    public class MetadataProfile : ModelBase
    {
        public string Name { get; set; }
        public double MinScore { get; set; }
        public bool SkipMissingDate { get; set; }
        public bool SkipPartsAndSets { get; set; }
        public bool SkipSeriesSecondary { get; set; }
        public string AllowedLanguages { get; set; }
        public int MinPages { get; set; }
    }
}