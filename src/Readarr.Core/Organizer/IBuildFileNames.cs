using Readarr.Core.Books;

namespace Readarr.Core.Organizer
{
    public interface IBuildFileNames
    {
        string GetAuthorFolder(Author author);
        string GetBookFolder(Book book);
        string GetBookFileName(Book book, BookFile bookFile);
    }
}