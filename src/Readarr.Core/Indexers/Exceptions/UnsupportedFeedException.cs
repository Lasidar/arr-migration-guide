using Readarr.Common.Exceptions;

namespace Readarr.Core.Indexers.Exceptions
{
    public class UnsupportedFeedException : ReadarrException
    {
        public UnsupportedFeedException(string message, params object[] args)
            : base(message, args)
        {
        }

        public UnsupportedFeedException(string message)
            : base(message)
        {
        }
    }
}
