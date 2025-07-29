using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles
{
    public interface IBookFileService
    {
        BookFile Add(BookFile bookFile);
        void Update(BookFile bookFile);
        void Delete(BookFile bookFile, DeleteMediaFileReason reason);
        List<BookFile> GetFilesByAuthor(int authorId);
        List<BookFile> GetFilesByBook(int bookId);
        List<BookFile> GetFilesByEdition(int editionId);
        BookFile GetFileByPath(string path);
        List<BookFile> GetFilesWithRelativePath(int authorId, string relativePath);
        List<BookFile> GetUnmappedFiles();
    }
}