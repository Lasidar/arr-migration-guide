using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Apprise
{
    public class AppriseException : ReadarrException
    {
        public AppriseException(string message)
            : base(message)
        {
        }

        public AppriseException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
