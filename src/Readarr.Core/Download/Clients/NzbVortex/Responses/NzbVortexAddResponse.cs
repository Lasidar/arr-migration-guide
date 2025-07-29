using Newtonsoft.Json;

namespace Readarr.Core.Download.Clients.NzbVortex.Responses
{
    public class NzbVortexAddResponse : NzbVortexResponseBase
    {
        [JsonProperty(PropertyName = "add_uuid")]
        public string Id { get; set; }
    }
}
