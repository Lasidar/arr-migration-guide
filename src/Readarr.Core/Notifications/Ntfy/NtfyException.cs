using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Ntfy
{
    public class NtfyException : ReadarrException
    {
        public NtfyException(string message)
            : base(message)
        {
        }

        public NtfyException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
