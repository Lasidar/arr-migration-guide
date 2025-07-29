using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Instrumentation.Commands
{
    public class DeleteUpdateLogFilesCommand : Command
    {
        public override bool SendUpdatesToClient => true;
    }
}
