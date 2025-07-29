using Readarr.Common.Messaging;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.ProgressMessaging
{
    public class CommandUpdatedEvent : IEvent
    {
        public CommandModel Command { get; set; }

        public CommandUpdatedEvent(CommandModel command)
        {
            Command = command;
        }
    }
}
