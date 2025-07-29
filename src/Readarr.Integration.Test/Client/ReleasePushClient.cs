using RestSharp;
using Readarr.Api.V3.Indexers;

namespace Readarr.Integration.Test.Client
{
    public class ReleasePushClient : ClientBase<ReleaseResource>
    {
        public ReleasePushClient(IRestClient restClient, string apiKey)
            : base(restClient, apiKey, "release/push")
        {
        }
    }
}
