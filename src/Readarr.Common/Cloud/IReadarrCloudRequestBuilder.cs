using Readarr.Common.Http;

namespace Readarr.Common.Cloud
{
    public interface IReadarrCloudRequestBuilder
    {
        IHttpRequestBuilderFactory BookInfo { get; }
        IHttpRequestBuilderFactory Search { get; }
    }
}