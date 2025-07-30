using Readarr.Common.Messaging;

namespace Readarr.Core.Lifecycle
{
    public class ApplicationShutdownRequested : IEvent
    {
        public bool Restarting { get; set; }
        
        public ApplicationShutdownRequested(bool restarting = false)
        {
            Restarting = restarting;
        }
    }
}
