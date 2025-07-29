using Readarr.Common.Exceptions;

namespace Readarr.Common.Disk
{
    public class NotParentException : ReadarrException
    {
        public NotParentException(string message, params object[] args)
            : base(message, args)
        {
        }

        public NotParentException(string message)
            : base(message)
        {
        }
    }
}
