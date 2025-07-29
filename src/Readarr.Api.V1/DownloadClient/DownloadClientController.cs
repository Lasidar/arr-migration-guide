using FluentValidation;
using Readarr.Core.Download;
using Readarr.SignalR;
using Readarr.Http;

namespace Readarr.Api.V1.DownloadClient
{
    [V3ApiController]
    public class DownloadClientController : ProviderControllerBase<DownloadClientResource, DownloadClientBulkResource, IDownloadClient, DownloadClientDefinition>
    {
        public static readonly DownloadClientResourceMapper ResourceMapper = new();
        public static readonly DownloadClientBulkResourceMapper BulkResourceMapper = new();

        public DownloadClientController(IBroadcastSignalRMessage signalRBroadcaster, IDownloadClientFactory downloadClientFactory)
            : base(signalRBroadcaster, downloadClientFactory, "downloadclient", ResourceMapper, BulkResourceMapper)
        {
            SharedValidator.RuleFor(c => c.Priority).InclusiveBetween(1, 50);
        }
    }
}
