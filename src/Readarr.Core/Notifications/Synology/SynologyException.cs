using Readarr.Common.Exceptions;

namespace Readarr.Core.Notifications.Synology
{
    public class SynologyException : ReadarrException
    {
        public SynologyException(string message)
            : base(message)
        {
        }

        public SynologyException(string message, params object[] args)
            : base(message, args)
        {
        }
    }
}
