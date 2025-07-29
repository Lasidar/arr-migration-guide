using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles
{
    public interface IMediaFileService
    {
        void Delete(BookFile bookFile);
        BookFile Get(int id);
        List<BookFile> Get(IEnumerable<int> ids);
        List<BookFile> GetFilesByAuthor(int authorId);
        List<BookFile> GetFilesByBook(int bookId);
        List<BookFile> GetUnmappedFiles();
        void UpdateMediaInfo(List<BookFile> bookFiles);
    }
}