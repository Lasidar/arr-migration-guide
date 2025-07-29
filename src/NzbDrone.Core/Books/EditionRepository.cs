using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NLog;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Qualities;

namespace NzbDrone.Core.Books
{
    public interface IEditionRepository : IBasicRepository<Episode>
    {
        Episode Find(int seriesId, int season, int episodeNumber);
        Episode Find(int seriesId, int absoluteEditionNumber);
        List<Episode> Find(int seriesId, string date);
        List<Episode> GetEpisodes(int seriesId);
        List<Episode> GetEpisodes(int seriesId, int seasonNumber);
        List<Episode> GetEpisodesByAuthorIds(List<int> seriesIds);
        List<Episode> GetEpisodesBySceneSeason(int seriesId, int sceneBookNumber);
        List<Episode> GetEpisodeByFileId(int fileId);
        List<Episode> EpisodesWithFiles(int seriesId);
        PagingSpec<Episode> EpisodesWithoutFiles(PagingSpec<Episode> pagingSpec, bool includeSpecials);
        PagingSpec<Episode> EpisodesWhereCutoffUnmet(PagingSpec<Episode> pagingSpec, List<QualitiesBelowCutoff> qualitiesBelowCutoff, bool includeSpecials);
        List<Episode> FindEpisodesBySceneNumbering(int seriesId, int seasonNumber, int episodeNumber);
        List<Episode> FindEpisodesBySceneNumbering(int seriesId, int sceneAbsoluteEditionNumber);
        List<Episode> EpisodesBetweenDates(DateTime startDate, DateTime endDate, bool includeUnmonitored);
        void SetMonitoredFlat(Episode episode, bool monitored);
        void SetMonitoredBySeason(int seriesId, int seasonNumber, bool monitored);
        void SetMonitored(IEnumerable<int> ids, bool monitored);
        void SetFileId(Episode episode, int fileId);
        void ClearFileId(Episode episode, bool unmonitor);
    }

    public class EditionRepository : BasicRepository<Episode>, IEditionRepository
    {
        private readonly Logger _logger;

        public EditionRepository(IMainDatabase database, IEventAggregator eventAggregator, Logger logger)
            : base(database, eventAggregator)
        {
            _logger = logger;
        }

        protected override IEnumerable<Episode> PagedQuery(SqlBuilder builder) =>
            _database.QueryJoined<Episode, Series>(builder, (episode, series) =>
            {
                episode.Series = series;
                return episode;
            });

        public Episode Find(int seriesId, int season, int episodeNumber)
        {
            return Query(s => s.AuthorId == seriesId && s.BookNumber == season && s.EditionNumber == episodeNumber)
                               .SingleOrDefault();
        }

        public Episode Find(int seriesId, int absoluteEditionNumber)
        {
            return Query(s => s.AuthorId == seriesId && s.AbsoluteEditionNumber == absoluteEditionNumber)
                        .SingleOrDefault();
        }

        public List<Episode> Find(int seriesId, string date)
        {
            return Query(s => s.AuthorId == seriesId && s.AirDate == date).ToList();
        }

        public List<Episode> GetEpisodes(int seriesId)
        {
            return Query(s => s.AuthorId == seriesId).ToList();
        }

        public List<Episode> GetEpisodes(int seriesId, int seasonNumber)
        {
            return Query(s => s.AuthorId == seriesId && s.BookNumber == seasonNumber).ToList();
        }

        public List<Episode> GetEpisodesByAuthorIds(List<int> seriesIds)
        {
            return Query(s => seriesIds.Contains(s.AuthorId)).ToList();
        }

        public List<Episode> GetEpisodesBySceneSeason(int seriesId, int seasonNumber)
        {
            return Query(s => s.AuthorId == seriesId && s.SceneBookNumber == seasonNumber).ToList();
        }

        public List<Episode> GetEpisodeByFileId(int fileId)
        {
            return Query(e => e.EditionFileId == fileId).ToList();
        }

        public List<Episode> EpisodesWithFiles(int seriesId)
        {
            var builder = Builder()
                .Join<Episode, EditionFile>((e, ef) => e.EditionFileId == ef.Id)
                .Where<Episode>(e => e.AuthorId == seriesId);

            return _database.QueryJoined<Episode, EditionFile>(
                builder,
                (episode, episodeFile) =>
                {
                    episode.EditionFile = episodeFile;
                    return episode;
                }).ToList();
        }

        public PagingSpec<Episode> EpisodesWithoutFiles(PagingSpec<Episode> pagingSpec, bool includeSpecials)
        {
            var currentTime = DateTime.UtcNow;
            var startingBookNumber = 1;

            if (includeSpecials)
            {
                startingBookNumber = 0;
            }

            pagingSpec.Records = GetPagedRecords(EpisodesWithoutFilesBuilder(currentTime, startingBookNumber), pagingSpec, PagedQuery);
            pagingSpec.TotalRecords = GetPagedRecordCount(EpisodesWithoutFilesBuilder(currentTime, startingBookNumber).SelectCountDistinct<Episode>(x => x.Id), pagingSpec);

            return pagingSpec;
        }

        public PagingSpec<Episode> EpisodesWhereCutoffUnmet(PagingSpec<Episode> pagingSpec, List<QualitiesBelowCutoff> qualitiesBelowCutoff, bool includeSpecials)
        {
            var startingBookNumber = 1;

            if (includeSpecials)
            {
                startingBookNumber = 0;
            }

            pagingSpec.Records = GetPagedRecords(EpisodesWhereCutoffUnmetBuilder(qualitiesBelowCutoff, startingBookNumber), pagingSpec, PagedQuery);

            var countTemplate = $"SELECT COUNT(*) FROM (SELECT /**select**/ FROM \"{TableMapping.Mapper.TableNameMapping(typeof(Episode))}\" /**join**/ /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**having**/) AS \"Inner\"";
            pagingSpec.TotalRecords = GetPagedRecordCount(EpisodesWhereCutoffUnmetBuilder(qualitiesBelowCutoff, startingBookNumber).Select(typeof(Episode)), pagingSpec, countTemplate);

            return pagingSpec;
        }

        public List<Episode> FindEpisodesBySceneNumbering(int seriesId, int seasonNumber, int episodeNumber)
        {
            return Query(s => s.AuthorId == seriesId && s.SceneBookNumber == seasonNumber && s.SceneEditionNumber == episodeNumber).ToList();
        }

        public List<Episode> FindEpisodesBySceneNumbering(int seriesId, int sceneAbsoluteEditionNumber)
        {
            return Query(s => s.AuthorId == seriesId && s.SceneAbsoluteEditionNumber == sceneAbsoluteEditionNumber).ToList();
        }

        public List<Episode> EpisodesBetweenDates(DateTime startDate, DateTime endDate, bool includeUnmonitored)
        {
            var builder = Builder().Where<Episode>(rg => rg.AirDateUtc >= startDate && rg.AirDateUtc <= endDate);

            if (!includeUnmonitored)
            {
                builder = builder.Where<Episode>(e => e.Monitored == true)
                    .Join<Episode, Series>((l, r) => l.AuthorId == r.Id)
                    .Where<Series>(e => e.Monitored == true);
            }

            return Query(builder);
        }

        public void SetMonitoredFlat(Episode episode, bool monitored)
        {
            episode.Monitored = monitored;
            SetFields(episode, p => p.Monitored);

            ModelUpdated(episode, true);
        }

        public void SetMonitoredBySeason(int seriesId, int seasonNumber, bool monitored)
        {
            using (var conn = _database.OpenConnection())
            {
                conn.Execute("UPDATE \"Episodes\" SET \"Monitored\" = @monitored WHERE \"AuthorId\" = @seriesId AND \"BookNumber\" = @seasonNumber AND \"Monitored\" != @monitored",
                    new { seriesId = seriesId, seasonNumber = seasonNumber, monitored = monitored });
            }
        }

        public void SetMonitored(IEnumerable<int> ids, bool monitored)
        {
            var episodes = ids.Select(x => new Episode { Id = x, Monitored = monitored }).ToList();
            SetFields(episodes, p => p.Monitored);
        }

        public void SetFileId(Episode episode, int fileId)
        {
            episode.EditionFileId = fileId;

            SetFields(episode, ep => ep.EditionFileId);

            ModelUpdated(episode, true);
        }

        public void ClearFileId(Episode episode, bool unmonitor)
        {
            episode.EditionFileId = 0;
            episode.Monitored &= !unmonitor;

            SetFields(episode, ep => ep.EditionFileId, ep => ep.Monitored);

            ModelUpdated(episode, true);
        }

        private SqlBuilder EpisodesWithoutFilesBuilder(DateTime currentTime, int startingBookNumber) => Builder()
            .Join<Episode, Series>((l, r) => l.AuthorId == r.Id)
            .Where<Episode>(f => f.EditionFileId == 0)
            .Where<Episode>(f => f.BookNumber >= startingBookNumber)
            .Where(BuildAirDateUtcCutoffWhereClause(currentTime));

        private string BuildAirDateUtcCutoffWhereClause(DateTime currentTime)
        {
            if (_database.DatabaseType == DatabaseType.PostgreSQL)
            {
                return string.Format("\"Episodes\".\"AirDateUtc\" + make_interval(mins => \"Series\".\"Runtime\") <= '{0}'",
                                     currentTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return string.Format("datetime(strftime('%s', \"Episodes\".\"AirDateUtc\") + \"Series\".\"Runtime\" * 60,  'unixepoch') <= '{0}'",
                                 currentTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private SqlBuilder EpisodesWhereCutoffUnmetBuilder(List<QualitiesBelowCutoff> qualitiesBelowCutoff, int startingBookNumber) => Builder()
            .Join<Episode, Series>((e, s) => e.AuthorId == s.Id)
            .LeftJoin<Episode, EditionFile>((e, ef) => e.EditionFileId == ef.Id)
            .Where<Episode>(e => e.EditionFileId != 0)
            .Where<Episode>(e => e.BookNumber >= startingBookNumber)
            .Where(
                string.Format("({0})",
                    BuildQualityCutoffWhereClause(qualitiesBelowCutoff)))
            .GroupBy<Episode>(e => e.Id)
            .GroupBy<Series>(s => s.Id);

        private string BuildQualityCutoffWhereClause(List<QualitiesBelowCutoff> qualitiesBelowCutoff)
        {
            var clauses = new List<string>();

            foreach (var profile in qualitiesBelowCutoff)
            {
                foreach (var belowCutoff in profile.QualityIds)
                {
                    clauses.Add(string.Format("(\"Series\".\"QualityProfileId\" = {0} AND \"EditionFiles\".\"Quality\" LIKE '%_quality_: {1},%')", profile.ProfileId, belowCutoff));
                }
            }

            return string.Format("({0})", string.Join(" OR ", clauses));
        }

        private Episode FindOneByAirDate(int seriesId, string date)
        {
            var episodes = Query(s => s.AuthorId == seriesId && s.AirDate == date).ToList();

            if (!episodes.Any())
            {
                return null;
            }

            if (episodes.Count == 1)
            {
                return episodes.First();
            }

            _logger.Debug("Multiple episodes with the same air date were found, will exclude specials");

            var regularEpisodes = episodes.Where(e => e.BookNumber > 0).ToList();

            if (regularEpisodes.Count == 1)
            {
                _logger.Debug("Left with one episode after excluding specials");
                return regularEpisodes.First();
            }

            throw new InvalidOperationException("Multiple episodes with the same air date found");
        }
    }
}
