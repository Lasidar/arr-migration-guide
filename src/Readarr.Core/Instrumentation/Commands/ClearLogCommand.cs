using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Instrumentation.Commands
{
    public class ClearLogCommand : Command
    {
        public override bool SendUpdatesToClient => true;
    }
}
