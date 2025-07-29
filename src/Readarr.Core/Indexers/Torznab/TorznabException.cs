using Readarr.Common.Exceptions;

namespace Readarr.Core.Indexers.Torznab
{
    public class TorznabException : ReadarrException
    {
        public TorznabException(string message, params object[] args)
            : base(message, args)
        {
        }

        public TorznabException(string message)
            : base(message)
        {
        }
    }
}
