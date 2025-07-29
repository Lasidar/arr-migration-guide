using Readarr.Common.Exceptions;

namespace Readarr.Core.Messaging.Commands
{
    public class CommandNotFoundException : ReadarrException
    {
        public CommandNotFoundException(string contract)
            : base("Couldn't find command " + contract)
        {
        }
    }
}
