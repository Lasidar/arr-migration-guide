using Newtonsoft.Json;
using Readarr.Core.Download.Clients.NzbVortex.JsonConverters;

namespace Readarr.Core.Download.Clients.NzbVortex.Responses
{
    public class NzbVortexResponseBase
    {
        [JsonConverter(typeof(NzbVortexResultTypeConverter))]
        public NzbVortexResultType Result { get; set; }
    }
}
