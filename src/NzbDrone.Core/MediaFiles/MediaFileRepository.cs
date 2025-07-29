using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.MediaFiles
{
    public interface IMediaFileRepository : IBasicRepository<EditionFile>
    {
        List<EditionFile> GetFilesBySeries(int seriesId);
        List<EditionFile> GetFilesByAuthorIds(List<int> seriesIds);
        List<EditionFile> GetFilesBySeason(int seriesId, int seasonNumber);
        List<EditionFile> GetFilesWithoutMediaInfo();
        List<EditionFile> GetFilesWithRelativePath(int seriesId, string relativePath);
        void DeleteForSeries(List<int> seriesIds);
    }

    public class MediaFileRepository : BasicRepository<EditionFile>, IMediaFileRepository
    {
        public MediaFileRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public List<EditionFile> GetFilesBySeries(int seriesId)
        {
            return Query(c => c.AuthorId == seriesId).ToList();
        }

        public List<EditionFile> GetFilesByAuthorIds(List<int> seriesIds)
        {
            return Query(c => seriesIds.Contains(c.AuthorId)).ToList();
        }

        public List<EditionFile> GetFilesBySeason(int seriesId, int seasonNumber)
        {
            return Query(c => c.AuthorId == seriesId && c.BookNumber == seasonNumber).ToList();
        }

        public List<EditionFile> GetFilesWithoutMediaInfo()
        {
            return Query(c => c.MediaInfo == null).ToList();
        }

        public List<EditionFile> GetFilesWithRelativePath(int seriesId, string relativePath)
        {
            return Query(c => c.AuthorId == seriesId && c.RelativePath == relativePath)
                        .ToList();
        }

        public void DeleteForSeries(List<int> seriesIds)
        {
            Delete(x => seriesIds.Contains(x.AuthorId));
        }
    }
}
