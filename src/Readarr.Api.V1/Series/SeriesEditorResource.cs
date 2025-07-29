using System.Collections.Generic;
using NzbDrone.Core.Books;

namespace Readarr.Api.V3.Series
{
    public class AuthorEditorResource
    {
        public List<int> AuthorIds { get; set; }
        public bool? Monitored { get; set; }
        public NewItemMonitorTypes? MonitorNewItems { get; set; }
        public int? QualityProfileId { get; set; }
        public SeriesTypes? SeriesType { get; set; }
        public bool? SeasonFolder { get; set; }
        public string RootFolderPath { get; set; }
        public List<int> Tags { get; set; }
        public ApplyTags ApplyTags { get; set; }
        public bool MoveFiles { get; set; }
        public bool DeleteFiles { get; set; }
        public bool AddImportListExclusion { get; set; }
    }
}
