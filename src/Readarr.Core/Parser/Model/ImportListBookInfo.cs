using System;
using Readarr.Core.Datastore;

namespace Readarr.Core.Parser.Model
{
    public class ImportListBookInfo : ModelBase
    {
        public int ImportListId { get; set; }
        public string ImportList { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public int Year { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public string GoodreadsId { get; set; }
        public string AuthorGoodreadsId { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Publisher { get; set; }
        public string Edition { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2}", ReleaseDate, AuthorName, Title);
        }
    }
}