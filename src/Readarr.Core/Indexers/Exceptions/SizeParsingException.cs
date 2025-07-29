using Readarr.Common.Exceptions;

namespace Readarr.Core.Indexers.Exceptions
{
    public class SizeParsingException : ReadarrException
    {
        public SizeParsingException(string message, params object[] args)
            : base(message, args)
        {
        }
    }
}
