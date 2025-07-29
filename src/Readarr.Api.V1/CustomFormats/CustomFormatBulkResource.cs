using System.Collections.Generic;

namespace Readarr.Api.V1.CustomFormats
{
    public class CustomFormatBulkResource
    {
        public HashSet<int> Ids { get; set; } = new();
        public bool? IncludeCustomFormatWhenRenaming { get; set; }
    }
}
