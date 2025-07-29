using Readarr.Core.Datastore;

namespace Readarr.Core.Tv
{
    public class Ratings : IEmbeddedDocument
    {
        public int Votes { get; set; }
        public decimal Value { get; set; }
    }
}
