using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Download;
using Readarr.Core.Indexers;
using Readarr.Core.Localization;
using Readarr.Core.ThingiProvider.Events;

namespace Readarr.Core.HealthCheck.Checks
{
    [CheckOn(typeof(ProviderUpdatedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderDeletedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderUpdatedEvent<IDownloadClient>))]
    [CheckOn(typeof(ProviderDeletedEvent<IDownloadClient>))]
    public class IndexerDownloadClientCheck : HealthCheckBase
    {
        private readonly IIndexerFactory _indexerFactory;
        private readonly IDownloadClientFactory _downloadClientFactory;

        public IndexerDownloadClientCheck(IIndexerFactory indexerFactory,
                                          IDownloadClientFactory downloadClientFactory,
                                          ILocalizationService localizationService)
            : base(localizationService)
        {
            _indexerFactory = indexerFactory;
            _downloadClientFactory = downloadClientFactory;
        }

        public override HealthCheck Check()
        {
            var downloadClientsIds = _downloadClientFactory.All().Where(v => v.Enable).Select(v => v.Id).ToList();
            var invalidIndexers = _indexerFactory.All()
                .Where(v => v.Enable && v.DownloadClientId > 0 && !downloadClientsIds.Contains(v.DownloadClientId))
                .ToList();

            if (invalidIndexers.Any())
            {
                return new HealthCheck(GetType(),
                    HealthCheckResult.Warning,
                    _localizationService.GetLocalizedString("IndexerDownloadClientHealthCheckMessage", new Dictionary<string, object>
                    {
                        { "indexerNames", string.Join(", ", invalidIndexers.Select(v => v.Name).ToArray()) }
                    }),
                    "#invalid-indexer-download-client-setting");
            }

            return new HealthCheck(GetType());
        }
    }
}
