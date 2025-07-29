using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Configuration
{
    public class InvalidConfigFileException : ReadarrException
    {
        public InvalidConfigFileException(string message)
            : base(message)
        {
        }

        public InvalidConfigFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
