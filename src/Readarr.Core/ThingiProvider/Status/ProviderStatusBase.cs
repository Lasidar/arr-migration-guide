using System;
using Readarr.Core.Datastore;

namespace Readarr.Core.ThingiProvider.Status
{
    public abstract class ProviderStatusBase : ModelBase
    {
        public int ProviderId { get; set; }

        public DateTime? InitialFailure { get; set; }
        public DateTime? MostRecentFailure { get; set; }
        public int EscalationLevel { get; set; }
        public DateTime? DisabledTill { get; set; }

        public virtual bool IsDisabled()
        {
            return DisabledTill.HasValue && DisabledTill.Value > DateTime.UtcNow;
        }
    }
}
