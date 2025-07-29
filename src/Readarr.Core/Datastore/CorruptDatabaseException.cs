using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Datastore
{
    public class CorruptDatabaseException : SonarrStartupException
    {
        public CorruptDatabaseException(string message, params object[] args)
            : base(message, args)
        {
        }

        public CorruptDatabaseException(string message)
            : base(message)
        {
        }

        public CorruptDatabaseException(string message, Exception innerException, params object[] args)
            : base(innerException, message, args)
        {
        }

        public CorruptDatabaseException(string message, Exception innerException)
            : base(innerException, message)
        {
        }
    }
}
