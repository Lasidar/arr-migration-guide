using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore.Events;
using Readarr.Core.Download;
using Readarr.Core.Download.Clients;
using Readarr.Core.Localization;
using Readarr.Core.RemotePathMappings;
using Readarr.Core.RootFolders;
using Readarr.Core.ThingiProvider.Events;

namespace Readarr.Core.HealthCheck.Checks
{
    [CheckOn(typeof(ProviderUpdatedEvent<IDownloadClient>))]
    [CheckOn(typeof(ProviderDeletedEvent<IDownloadClient>))]
    [CheckOn(typeof(ModelEvent<RootFolder>))]
    [CheckOn(typeof(ModelEvent<RemotePathMapping>))]

    public class DownloadClientRootFolderCheck : HealthCheckBase, IProvideHealthCheck
    {
        private readonly IProvideDownloadClient _downloadClientProvider;
        private readonly IRootFolderService _rootFolderService;
        private readonly Logger _logger;

        public DownloadClientRootFolderCheck(IProvideDownloadClient downloadClientProvider,
                                             IRootFolderService rootFolderService,
                                             Logger logger,
                                             ILocalizationService localizationService)
            : base(localizationService)
        {
            _downloadClientProvider = downloadClientProvider;
            _rootFolderService = rootFolderService;
            _logger = logger;
        }

        public override HealthCheck Check()
        {
            // Only check clients not in failure status, those get another message
            var clients = _downloadClientProvider.GetDownloadClients(true);

            var rootFolders = _rootFolderService.All();

            foreach (var client in clients)
            {
                try
                {
                    var status = client.GetStatus();
                    var folders = rootFolders.Where(r => status.OutputRootFolders.Any(folder => r.Path.PathEquals(folder.FullPath) || r.Path.IsParentPath(folder.FullPath)));

                    foreach (var folder in folders)
                    {
                        return new HealthCheck(GetType(),
                            HealthCheckResult.Warning,
                            _localizationService.GetLocalizedString("DownloadClientRootFolderHealthCheckMessage", new Dictionary<string, object>
                            {
                                { "downloadClientName", client.Definition.Name },
                                { "rootFolderPath", folder.Path }
                            }),
                            "#downloads-in-root-folder");
                    }
                }
                catch (DownloadClientException ex)
                {
                    _logger.Debug(ex, "Unable to communicate with {0}", client.Definition.Name);
                }
                catch (HttpRequestException ex)
                {
                    _logger.Debug(ex, "Unable to communicate with {0}", client.Definition.Name);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unknown error occurred in DownloadClientRootFolderCheck HealthCheck");
                }
            }

            return new HealthCheck(GetType());
        }
    }
}
