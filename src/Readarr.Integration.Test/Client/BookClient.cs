using System.Collections.Generic;
using Readarr.Api.V1.Books;
using RestSharp;

namespace Readarr.Integration.Test.Client
{
    public class BookClient : ClientBase<BookResource>
    {
        public BookClient(IRestClient restClient, string apiKey)
            : base(restClient, apiKey, "book")
        {
        }

        public List<BookResource> GetBooksInAuthor(int authorId)
        {
            var request = BuildRequest("?authorId=" + authorId.ToString());
            return Get<List<BookResource>>(request);
        }
    }
}