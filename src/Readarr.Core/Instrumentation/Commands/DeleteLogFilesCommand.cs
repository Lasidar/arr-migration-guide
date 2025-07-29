using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Instrumentation.Commands
{
    public class DeleteLogFilesCommand : Command
    {
        public override bool SendUpdatesToClient => true;
    }
}
