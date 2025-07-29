using System;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Jobs
{
    public class ScheduledTask : ModelBase
    {
        public string TypeName { get; set; }
        public int Interval { get; set; }
        public DateTime LastExecution { get; set; }
        public CommandPriority Priority { get; set; }
        public DateTime LastStartTime { get; set; }

        public ScheduledTask()
        {
            Priority = CommandPriority.Low;
        }
    }
}
