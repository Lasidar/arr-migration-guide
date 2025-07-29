using System;

namespace Readarr.Common.Exceptions
{
    public abstract class ReadarrException : ApplicationException
    {
        protected ReadarrException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        protected ReadarrException(string message)
            : base(message)
        {
        }

        protected ReadarrException(string message, Exception innerException, params object[] args)
            : base(string.Format(message, args), innerException)
        {
        }

        protected ReadarrException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
