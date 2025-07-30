using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists
{
    public class ImportListBookFetchResult
    {
        public ImportListBookFetchResult()
        {
            Books = new List<ImportListBookInfo>();
        }

        public ImportListBookFetchResult(IEnumerable<ImportListBookInfo> books, bool anyFailure)
        {
            Books = books.ToList();
            AnyFailure = anyFailure;
        }

        public List<ImportListBookInfo> Books { get; set; }
        public bool AnyFailure { get; set; }
    }
}