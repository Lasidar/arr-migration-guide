using Readarr.Core.Notifications.Trakt.Resource;

namespace Readarr.Core.Notifications.Trakt
{
    public class TraktUserResource
    {
        public string Username { get; set; }
        public TraktUserIdsResource Ids { get; set; }
    }
}
