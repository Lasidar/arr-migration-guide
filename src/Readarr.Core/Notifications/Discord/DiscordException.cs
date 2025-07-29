using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Discord
{
    public class DiscordException : ReadarrException
    {
        public DiscordException(string message)
            : base(message)
        {
        }

        public DiscordException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
