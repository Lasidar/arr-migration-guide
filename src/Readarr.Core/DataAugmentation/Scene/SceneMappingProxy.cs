using System.Collections.Generic;
using Readarr.Common.Cloud;
using Readarr.Common.Http;

namespace Readarr.Core.DataAugmentation.Scene
{
    public interface ISceneMappingProxy
    {
        List<SceneMapping> Fetch();
    }

    public class SceneMappingProxy : ISceneMappingProxy
    {
        private readonly IHttpClient _httpClient;
        private readonly IHttpRequestBuilderFactory _requestBuilder;

        public SceneMappingProxy(IHttpClient httpClient, IReadarrCloudRequestBuilder requestBuilder)
        {
            _httpClient = httpClient;
            _requestBuilder = requestBuilder.Services;
        }

        public List<SceneMapping> Fetch()
        {
            var request = _requestBuilder.Create()
                                         .Resource("/scenemapping")
                                         .Build();

            return _httpClient.Get<List<SceneMapping>>(request).Resource;
        }
    }
}
