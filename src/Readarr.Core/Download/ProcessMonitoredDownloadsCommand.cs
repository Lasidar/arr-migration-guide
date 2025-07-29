using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Download
{
    public class ProcessMonitoredDownloadsCommand : Command
    {
        public override bool RequiresDiskAccess => true;

        public override bool IsLongRunning => true;
    }
}
