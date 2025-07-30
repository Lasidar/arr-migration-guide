using System;

namespace Readarr.Core.Backup
{
    public class BackupException : Exception
    {
        public BackupException(string message) : base(message)
        {
        }

        public BackupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}