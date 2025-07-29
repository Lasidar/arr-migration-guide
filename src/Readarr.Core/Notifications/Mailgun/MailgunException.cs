using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Mailgun
{
    public class MailgunException : ReadarrException
    {
        public MailgunException(string message)
            : base(message)
        {
        }

        public MailgunException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
