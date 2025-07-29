using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Indexers;
using Readarr.Core.Indexers.Torznab;
using Readarr.Core.Localization;
using Readarr.Core.ThingiProvider.Events;

namespace Readarr.Core.HealthCheck.Checks
{
    [CheckOn(typeof(ProviderAddedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderUpdatedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderDeletedEvent<IIndexer>))]
    [CheckOn(typeof(ProviderStatusChangedEvent<IIndexer>))]
    public class IndexerJackettAllCheck : HealthCheckBase
    {
        private readonly IIndexerFactory _providerFactory;

        public IndexerJackettAllCheck(IIndexerFactory providerFactory, ILocalizationService localizationService)
            : base(localizationService)
        {
            _providerFactory = providerFactory;
        }

        public override HealthCheck Check()
        {
            var jackettAllProviders = _providerFactory.All()
                .Where(
                    i => i.Enable &&
                         i.ConfigContract.Equals("TorznabSettings") &&
                         (((TorznabSettings)i.Settings).BaseUrl.Contains("/torznab/all/api", StringComparison.InvariantCultureIgnoreCase) ||
                          ((TorznabSettings)i.Settings).BaseUrl.Contains("/api/v2.0/indexers/all/results/torznab", StringComparison.InvariantCultureIgnoreCase) ||
                          ((TorznabSettings)i.Settings).ApiPath.Contains("/torznab/all/api", StringComparison.InvariantCultureIgnoreCase) ||
                          ((TorznabSettings)i.Settings).ApiPath.Contains("/api/v2.0/indexers/all/results/torznab", StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();

            if (jackettAllProviders.Empty())
            {
                return new HealthCheck(GetType());
            }

            return new HealthCheck(GetType(),
                HealthCheckResult.Warning,
                _localizationService.GetLocalizedString("IndexerJackettAllHealthCheckMessage", new Dictionary<string, object>
                {
                    { "indexerNames", string.Join(", ", jackettAllProviders.Select(i => i.Name)) }
                }),
                "#jackett-all-endpoint-used");
        }
    }
}
