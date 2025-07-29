using Readarr.Core.ImportLists;

namespace Readarr.Core.Housekeeping.Housekeepers
{
    public class FixFutureImportListStatusTimes : FixFutureProviderStatusTimes<ImportListStatus>, IHousekeepingTask
    {
        public FixFutureImportListStatusTimes(IImportListStatusRepository importListStatusRepository)
            : base(importListStatusRepository)
        {
        }
    }
}
