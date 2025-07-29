﻿using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Download
{
    public interface IDownloadClientRepository : IProviderRepository<DownloadClientDefinition>
    {
    }

    public class DownloadClientRepository : ProviderRepository<DownloadClientDefinition>, IDownloadClientRepository
    {
        public DownloadClientRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
