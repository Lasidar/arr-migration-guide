using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.MetadataSource
{
    public interface IProvideBookInfo
    {
        Book GetBookInfo(string foreignBookId);
        List<Book> GetBooksForAuthor(int authorMetadataId);
        List<Book> SearchForNewBook(string title);
        List<Book> SearchForNewBookByIsbn(string isbn);
    }
}