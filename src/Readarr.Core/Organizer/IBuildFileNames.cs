using Readarr.Core.Books;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Organizer
{
    public interface IBuildFileNames
    {
        string GetAuthorFolder(Author author);
        string GetBookFolder(Book book);
        string GetBookFileName(Book book, BookFile bookFile);
    }
}