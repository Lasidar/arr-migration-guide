using Readarr.Common.Messaging;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Messaging.Events
{
    public class CommandExecutedEvent : IEvent
    {
        public CommandModel Command { get; private set; }

        public CommandExecutedEvent(CommandModel command)
        {
            Command = command;
        }
    }
}
