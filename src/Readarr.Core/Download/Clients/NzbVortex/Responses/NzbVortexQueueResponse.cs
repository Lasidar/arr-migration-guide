using System.Collections.Generic;
using Newtonsoft.Json;

namespace Readarr.Core.Download.Clients.NzbVortex.Responses
{
    public class NzbVortexQueueResponse : NzbVortexResponseBase
    {
        [JsonProperty(PropertyName = "nzbs")]
        public List<NzbVortexQueueItem> Items { get; set; }
    }
}
