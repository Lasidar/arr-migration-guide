using Readarr.Common.Exceptions;

namespace Readarr.Core.Exceptions
{
    public class SearchFailedException : ReadarrException
    {
        public SearchFailedException(string message)
            : base(message)
        {
        }
    }
}
