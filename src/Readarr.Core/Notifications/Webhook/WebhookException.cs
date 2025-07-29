using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookException : ReadarrException
    {
        public WebhookException(string message)
            : base(message)
        {
        }

        public WebhookException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
