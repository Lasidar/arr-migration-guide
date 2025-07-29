using Readarr.Common.Exceptions;

namespace Readarr.Core.Parser
{
    public class InvalidDateException : ReadarrException
    {
        public InvalidDateException(string message, params object[] args)
            : base(message, args)
        {
        }

        public InvalidDateException(string message)
            : base(message)
        {
        }
    }
}
