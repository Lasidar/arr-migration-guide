using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Common.Http;

namespace Readarr.Core.ImportLists.Simkl.User
{
    public class SimklUserRequestGenerator : IImportListRequestGenerator
    {
        public SimklUserSettings Settings { get; set; }

        public string ClientId { get; set; }

        public virtual ImportListPageableRequestChain GetListItems()
        {
            var pageableRequests = new ImportListPageableRequestChain();

            pageableRequests.Add(GetSeriesRequest());

            return pageableRequests;
        }

        private IEnumerable<ImportListRequest> GetSeriesRequest()
        {
            var link = $"{Settings.BaseUrl.Trim()}/sync/all-items/{((SimklUserShowType)Settings.ShowType).ToString().ToLowerInvariant()}/{((SimklUserListType)Settings.ListType).ToString().ToLowerInvariant()}";

            var request = new ImportListRequest(link, HttpAccept.Json);

            request.HttpRequest.Headers.Add("simkl-api-key", ClientId);

            if (Settings.AccessToken.IsNotNullOrWhiteSpace())
            {
                request.HttpRequest.Headers.Add("Authorization", "Bearer " + Settings.AccessToken);
            }

            yield return request;
        }
    }
}
