using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Readarr.Core.Tv;

namespace Readarr.Core.Notifications.Webhook
{
    // TODO: In v4 this will likely be changed to the default camel case.
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(DefaultNamingStrategy))]
    public enum WebhookEventType
    {
        Test,
        Grab,
        Download,
        Rename,
        
        // Book events
        AuthorAdd,
        AuthorDelete,
        BookFileDelete,
        
        // TV events (to be removed)
        SeriesAdd,
        SeriesDelete,
        EpisodeFileDelete,
        
        // Common events
        Health,
        ApplicationUpdate,
        HealthRestored,
        ManualInteractionRequired
    }
}
