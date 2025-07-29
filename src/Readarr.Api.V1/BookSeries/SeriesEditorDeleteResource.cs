using System.Collections.Generic;

namespace Readarr.Api.V1.Series
{
    public class SeriesEditorDeleteResource
    {
        public List<int> SeriesIds { get; set; }
        public bool DeleteFiles { get; set; }
    }
}
