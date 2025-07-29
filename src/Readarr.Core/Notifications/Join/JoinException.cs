using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Join
{
    public class JoinException : ReadarrException
    {
        public JoinException(string message)
            : base(message)
        {
        }

        public JoinException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }
    }
}
