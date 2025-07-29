using Readarr.Common.Messaging;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.ThingiProvider.Events
{
    public class ProviderStatusChangedEvent<TProvider> : IEvent
    {
        public int ProviderId { get; private set; }

        public ProviderStatusBase Status { get; private set; }

        public ProviderStatusChangedEvent(int id, ProviderStatusBase status)
        {
            ProviderId = id;
            Status = status;
        }
    }
}
