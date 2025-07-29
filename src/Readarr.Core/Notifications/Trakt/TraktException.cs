using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Trakt
{
    public class TraktException : ReadarrException
    {
        public TraktException(string message)
            : base(message)
        {
        }

        public TraktException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
