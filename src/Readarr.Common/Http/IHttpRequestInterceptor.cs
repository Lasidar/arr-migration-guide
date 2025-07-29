namespace Readarr.Common.Http
{
    public interface IHttpRequestInterceptor
    {
        HttpRequest PreRequest(HttpRequest request);
        HttpResponse PostResponse(HttpResponse response);
    }
}
