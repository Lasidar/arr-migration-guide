using RestSharp;
using Readarr.Api.V3.Indexers;

namespace Readarr.Integration.Test.Client
{
    public class ReleaseClient : ClientBase<ReleaseResource>
    {
        public ReleaseClient(IRestClient restClient, string apiKey)
            : base(restClient, apiKey)
        {
        }
    }
}
