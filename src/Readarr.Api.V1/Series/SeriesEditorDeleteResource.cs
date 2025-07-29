using System.Collections.Generic;

namespace Readarr.Api.V3.Series
{
    public class AuthorEditorDeleteResource
    {
        public List<int> AuthorIds { get; set; }
        public bool DeleteFiles { get; set; }
    }
}
