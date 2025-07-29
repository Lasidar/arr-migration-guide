using Readarr.Common.Exceptions;

namespace Readarr.Core.Indexers.Exceptions
{
    public class ApiKeyException : ReadarrException
    {
        public ApiKeyException(string message, params object[] args)
            : base(message, args)
        {
        }

        public ApiKeyException(string message)
            : base(message)
        {
        }
    }
}
