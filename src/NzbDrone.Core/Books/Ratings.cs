using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Books
{
    public class Ratings : IEmbeddedDocument
    {
        public int Votes { get; set; }
        public decimal Value { get; set; }
    }
}
