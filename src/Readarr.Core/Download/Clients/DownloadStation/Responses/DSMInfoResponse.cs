using Newtonsoft.Json;

namespace Readarr.Core.Download.Clients.DownloadStation.Responses
{
    public class DSMInfoResponse
    {
        [JsonProperty("serial")]
        public string SerialNumber { get; set; }
    }
}
