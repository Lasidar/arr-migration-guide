using Newtonsoft.Json;

namespace Readarr.Core.Notifications.Discord.Payloads
{
    public class DiscordAuthor
    {
        public string Name { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
    }
}
