using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.Extras.Files
{
    public interface IExtraFileRepository<TExtraFile> : IBasicRepository<TExtraFile>
        where TExtraFile : ExtraFile, new()
    {
        void DeleteForAuthorIds(List<int> seriesIds);
        void DeleteForSeason(int seriesId, int seasonNumber);
        void DeleteForEditionFile(int episodeFileId);
        List<TExtraFile> GetFilesBySeries(int seriesId);
        List<TExtraFile> GetFilesBySeason(int seriesId, int seasonNumber);
        List<TExtraFile> GetFilesByEditionFile(int episodeFileId);
        TExtraFile FindByPath(int seriesId, string path);
    }

    public class ExtraFileRepository<TExtraFile> : BasicRepository<TExtraFile>, IExtraFileRepository<TExtraFile>
        where TExtraFile : ExtraFile, new()
    {
        public ExtraFileRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public void DeleteForAuthorIds(List<int> seriesIds)
        {
            Delete(c => seriesIds.Contains(c.AuthorId));
        }

        public void DeleteForSeason(int seriesId, int seasonNumber)
        {
            Delete(c => c.AuthorId == seriesId && c.BookNumber == seasonNumber);
        }

        public void DeleteForEditionFile(int episodeFileId)
        {
            Delete(c => c.EditionFileId == episodeFileId);
        }

        public List<TExtraFile> GetFilesBySeries(int seriesId)
        {
            return Query(c => c.AuthorId == seriesId);
        }

        public List<TExtraFile> GetFilesBySeason(int seriesId, int seasonNumber)
        {
            return Query(c => c.AuthorId == seriesId && c.BookNumber == seasonNumber);
        }

        public List<TExtraFile> GetFilesByEditionFile(int episodeFileId)
        {
            return Query(c => c.EditionFileId == episodeFileId);
        }

        public TExtraFile FindByPath(int seriesId, string path)
        {
            return Query(c => c.AuthorId == seriesId && c.RelativePath == path).SingleOrDefault();
        }
    }
}
