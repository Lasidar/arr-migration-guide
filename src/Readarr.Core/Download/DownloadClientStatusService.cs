using System;
using NLog;
using Readarr.Common.EnvironmentInfo;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.Download
{
    public interface IDownloadClientStatusService : IProviderStatusServiceBase<DownloadClientStatus>
    {
    }

    public class DownloadClientStatusService : ProviderStatusServiceBase<IDownloadClient, DownloadClientStatus>, IDownloadClientStatusService
    {
        public DownloadClientStatusService(IDownloadClientStatusRepository providerStatusRepository, IEventAggregator eventAggregator, IRuntimeInfo runtimeInfo, Logger logger)
            : base(providerStatusRepository, eventAggregator, runtimeInfo, logger)
        {
            MinimumTimeSinceInitialFailure = TimeSpan.FromMinutes(5);
            MaximumEscalationLevel = 5;
        }
    }
}
