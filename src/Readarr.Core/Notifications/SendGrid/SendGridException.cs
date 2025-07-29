using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.SendGrid
{
    public class SendGridException : ReadarrException
    {
        public SendGridException(string message)
            : base(message)
        {
        }

        public SendGridException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
