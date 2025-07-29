using Readarr.Core.Datastore;

namespace Readarr.Core.Books
{
    public class Ratings : IEmbeddedDocument
    {
        public int Votes { get; set; }
        public decimal Value { get; set; }
        public decimal? Popularity { get; set; } // For tracking book popularity
    }
}