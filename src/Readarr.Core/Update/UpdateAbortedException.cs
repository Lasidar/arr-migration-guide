using Readarr.Common.Exceptions;

namespace Readarr.Core.Update
{
    public class UpdateFailedException : ReadarrException
    {
        public UpdateFailedException(string message, params object[] args)
            : base(message, args)
        {
        }

        public UpdateFailedException(string message)
            : base(message)
        {
        }
    }
}
