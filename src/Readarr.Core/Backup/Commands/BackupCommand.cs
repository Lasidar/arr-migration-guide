using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Backup.Commands
{
    public class BackupCommand : Command
    {
        public BackupType Type { get; set; }

        public override bool RequiresDiskAccess => true;
        public override bool IsExclusive => true;
    }
}