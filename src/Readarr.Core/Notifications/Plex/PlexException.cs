using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Plex
{
    public class PlexException : ReadarrException
    {
        public PlexException(string message)
            : base(message)
        {
        }

        public PlexException(string message, params object[] args)
            : base(message, args)
        {
        }

        public PlexException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
