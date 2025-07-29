﻿using System;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Exceptions
{
    public class ReleaseUnavailableException : ReleaseDownloadException
    {
        public ReleaseUnavailableException(ReleaseInfo release, string message, params object[] args)
            : base(release, message, args)
        {
        }

        public ReleaseUnavailableException(ReleaseInfo release, string message)
            : base(release, message)
        {
        }

        public ReleaseUnavailableException(ReleaseInfo release, string message, Exception innerException, params object[] args)
            : base(release, message, innerException, args)
        {
        }

        public ReleaseUnavailableException(ReleaseInfo release, string message, Exception innerException)
            : base(release, message, innerException)
        {
        }
    }
}
