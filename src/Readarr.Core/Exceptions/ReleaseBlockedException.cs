using System;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Exceptions
{
    public class ReleaseBlockedException : ReleaseDownloadException
    {
        public ReleaseBlockedException(ReleaseInfo release, string message, params object[] args)
            : base(release, message, args)
        {
        }

        public ReleaseBlockedException(ReleaseInfo release, string message)
            : base(release, message)
        {
        }

        public ReleaseBlockedException(ReleaseInfo release, string message, Exception innerException, params object[] args)
            : base(release, message, innerException, args)
        {
        }

        public ReleaseBlockedException(ReleaseInfo release, string message, Exception innerException)
            : base(release, message, innerException)
        {
        }
    }
}
