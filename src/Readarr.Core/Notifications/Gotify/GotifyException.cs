using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Gotify
{
    public class GotifyException : ReadarrException
    {
        public GotifyException(string message)
            : base(message)
        {
        }

        public GotifyException(string message, params object[] args)
            : base(message, args)
        {
        }

        public GotifyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
