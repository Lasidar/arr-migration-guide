using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.MediaFiles
{
    public interface IBookFileRepository : IBasicRepository<BookFile>
    {
        List<BookFile> GetFilesByAuthor(int authorId);
        List<BookFile> GetFilesByBook(int bookId);
        List<BookFile> GetFilesByEdition(int editionId);
        List<BookFile> GetUnmappedFiles();
        List<BookFile> GetFilesWithRelativePath(int authorId, string relativePath);
        BookFile GetFileWithPath(string path);
    }

    public class BookFileRepository : BasicRepository<BookFile>, IBookFileRepository
    {
        public BookFileRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public List<BookFile> GetFilesByAuthor(int authorId)
        {
            return Query(f => f.AuthorId == authorId).ToList();
        }

        public List<BookFile> GetFilesByBook(int bookId)
        {
            return Query(f => f.BookId == bookId).ToList();
        }

        public List<BookFile> GetFilesByEdition(int editionId)
        {
            return Query(f => f.EditionId == editionId).ToList();
        }

        public List<BookFile> GetUnmappedFiles()
        {
            return Query(f => f.EditionId == 0).ToList();
        }

        public List<BookFile> GetFilesWithRelativePath(int authorId, string relativePath)
        {
            return Query(f => f.AuthorId == authorId && f.RelativePath == relativePath).ToList();
        }

        public BookFile GetFileWithPath(string path)
        {
            return Query(f => f.Path == path).SingleOrDefault();
        }
    }
}