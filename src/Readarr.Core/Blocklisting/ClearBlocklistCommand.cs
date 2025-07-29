using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Blocklisting
{
    public class ClearBlocklistCommand : Command
    {
        public override bool SendUpdatesToClient => true;
    }
}
