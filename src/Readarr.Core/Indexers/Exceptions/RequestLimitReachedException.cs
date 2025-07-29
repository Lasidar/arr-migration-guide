using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Indexers.Exceptions
{
    public class RequestLimitReachedException : ReadarrException
    {
        public TimeSpan RetryAfter { get; private set; }

        public RequestLimitReachedException(string message, params object[] args)
            : base(message, args)
        {
        }

        public RequestLimitReachedException(string message)
            : base(message)
        {
        }

        public RequestLimitReachedException(string message, TimeSpan retryAfter)
            : base(message)
        {
            RetryAfter = retryAfter;
        }
    }
}
