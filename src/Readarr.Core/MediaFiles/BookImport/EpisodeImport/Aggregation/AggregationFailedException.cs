using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.MediaFiles.BookImport.Aggregation
{
    public class AugmentingFailedException : ReadarrException
    {
        public AugmentingFailedException(string message, params object[] args)
            : base(message, args)
        {
        }

        public AugmentingFailedException(string message)
            : base(message)
        {
        }

        public AugmentingFailedException(string message, Exception innerException, params object[] args)
            : base(message, innerException, args)
        {
        }

        public AugmentingFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
