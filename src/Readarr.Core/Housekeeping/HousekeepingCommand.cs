using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Housekeeping
{
    public class HousekeepingCommand : Command
    {
        public override bool RequiresDiskAccess => true;
    }
}
