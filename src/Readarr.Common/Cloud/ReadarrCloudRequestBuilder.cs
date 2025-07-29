using Readarr.Common.Http;

namespace Readarr.Common.Cloud
{
    public class ReadarrCloudRequestBuilder : IReadarrCloudRequestBuilder
    {
        public ReadarrCloudRequestBuilder()
        {
            BookInfo = new HttpRequestBuilder("https://api.readarr.com/v1/bookinfo/")
                .CreateFactory();

            Search = new HttpRequestBuilder("https://api.readarr.com/v1/search/")
                .CreateFactory();
        }

        public IHttpRequestBuilderFactory BookInfo { get; }

        public IHttpRequestBuilderFactory Search { get; }
    }
}
