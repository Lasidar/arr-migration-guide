using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Update.Commands
{
    public class ApplicationUpdateCommand : Command
    {
        public bool InstallMajorUpdate { get; set; }
        public override bool SendUpdatesToClient => true;
        public override bool IsExclusive => true;
    }
}
