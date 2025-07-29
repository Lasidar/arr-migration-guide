using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Prowl
{
    public class ProwlException : ReadarrException
    {
        public ProwlException(string message)
            : base(message)
        {
        }

        public ProwlException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
