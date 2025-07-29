using Readarr.Common.Messaging;

namespace Readarr.Core.Backup
{
    public class BackupCompleteEvent : IEvent
    {
        public string BackupPath { get; private set; }

        public BackupCompleteEvent(string backupPath)
        {
            BackupPath = backupPath;
        }
    }
}