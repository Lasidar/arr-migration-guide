using Readarr.Common.Exceptions;

namespace Readarr.Core.Organizer
{
    public class NamingFormatException : ReadarrException
    {
        public NamingFormatException(string message, params object[] args)
            : base(message, args)
        {
        }

        public NamingFormatException(string message)
            : base(message)
        {
        }
    }
}
