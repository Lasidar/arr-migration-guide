using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles
{
    public interface IMediaFileService
    {
        // Book methods
        void Delete(BookFile bookFile);
        void Delete(BookFile bookFile, DeleteMediaFileReason reason);
        BookFile Get(int id);
        List<BookFile> Get(IEnumerable<int> ids);
        List<BookFile> GetFilesByAuthor(int authorId);
        List<BookFile> GetFilesByBook(int bookId);
        List<BookFile> GetUnmappedFiles();
        void UpdateMediaInfo(List<BookFile> bookFiles);
        
        // TV methods (for compatibility)
        List<EpisodeFile> GetFilesBySeries(int seriesId);
        List<EpisodeFile> GetFilesBySeason(int seriesId, int seasonNumber);
        List<EpisodeFile> GetFilesBySeriesIds(List<int> seriesIds);
        void Update(EpisodeFile episodeFile);
        void Update(BookFile bookFile);
        void Delete(EpisodeFile episodeFile, DeleteMediaFileReason reason);
        List<EpisodeFile> GetFilesWithRelativePath(int seriesId, string relativePath);
        List<string> FilterExistingFiles(List<string> files, Tv.Series series);
    }
}