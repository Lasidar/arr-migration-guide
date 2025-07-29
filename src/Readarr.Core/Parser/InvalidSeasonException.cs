using Readarr.Common.Exceptions;

namespace Readarr.Core.Parser
{
    public class InvalidSeasonException : ReadarrException
    {
        public InvalidSeasonException(string message, params object[] args)
            : base(message, args)
        {
        }

        public InvalidSeasonException(string message)
            : base(message)
        {
        }
    }
}
