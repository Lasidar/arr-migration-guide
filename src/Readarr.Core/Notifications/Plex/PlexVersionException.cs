using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Plex
{
    public class PlexVersionException : ReadarrException
    {
        public PlexVersionException(string message)
            : base(message)
        {
        }

        public PlexVersionException(string message, params object[] args)
            : base(message, args)
        {
        }
    }
}
