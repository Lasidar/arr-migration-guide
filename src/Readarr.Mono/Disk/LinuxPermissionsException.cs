using Readarr.Common.Exceptions;

namespace Readarr.Mono.Disk
{
    public class LinuxPermissionsException : ReadarrException
    {
        public LinuxPermissionsException(string message, params object[] args)
            : base(message, args)
        {
        }

        public LinuxPermissionsException(string message)
            : base(message)
        {
        }
    }
}
