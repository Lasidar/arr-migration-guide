using System.Net;
using System.Threading.Tasks;

namespace Readarr.Common.Http.Dispatchers
{
    public interface IHttpDispatcher
    {
        Task<HttpResponse> GetResponseAsync(HttpRequest request, CookieContainer cookies);
    }
}
