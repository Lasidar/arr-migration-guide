using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Notifiarr
{
    public class NotifiarrException : ReadarrException
    {
        public NotifiarrException(string message)
            : base(message)
        {
        }

        public NotifiarrException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
