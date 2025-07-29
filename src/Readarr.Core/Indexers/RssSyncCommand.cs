using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Indexers
{
    public class RssSyncCommand : Command
    {
        public override bool SendUpdatesToClient => true;
        public override bool IsLongRunning => true;
    }
}
