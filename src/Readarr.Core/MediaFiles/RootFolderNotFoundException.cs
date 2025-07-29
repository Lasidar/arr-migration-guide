using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.MediaFiles
{
    public class RootFolderNotFoundException : ReadarrException
    {
        public RootFolderNotFoundException(string message) : base(message)
        {
        }

        public RootFolderNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}