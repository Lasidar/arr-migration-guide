using System;

namespace Readarr.Core.MediaFiles
{
    public class DestinationAlreadyExistsException : Exception
    {
        public DestinationAlreadyExistsException(string message) : base(message)
        {
        }

        public DestinationAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}