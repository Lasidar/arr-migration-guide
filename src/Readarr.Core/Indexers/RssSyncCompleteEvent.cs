using Readarr.Common.Messaging;
using Readarr.Core.Download;

namespace Readarr.Core.Indexers
{
    public class RssSyncCompleteEvent : IEvent
    {
        public ProcessedDecisions ProcessedDecisions { get; private set; }

        public RssSyncCompleteEvent(ProcessedDecisions processedDecisions)
        {
            ProcessedDecisions = processedDecisions;
        }
    }
}
