using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.SeriesStats
{
    public interface ISeriesStatisticsRepository
    {
        List<SeasonStatistics> SeriesStatistics();
        List<SeasonStatistics> SeriesStatistics(int seriesId);
    }

    public class AuthorStatisticsRepository : ISeriesStatisticsRepository
    {
        private const string _selectEpisodesTemplate = "SELECT /**select**/ FROM \"Episodes\" /**join**/ /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**having**/ /**orderby**/";
        private const string _selectEditionFilesTemplate = "SELECT /**select**/ FROM \"EditionFiles\" /**join**/ /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**having**/ /**orderby**/";

        private readonly IMainDatabase _database;

        public SeriesStatisticsRepository(IMainDatabase database)
        {
            _database = database;
        }

        public List<SeasonStatistics> SeriesStatistics()
        {
            var time = DateTime.UtcNow;
            return MapResults(Query(EpisodesBuilder(time), _selectEpisodesTemplate),
                Query(EditionFilesBuilder(), _selectEditionFilesTemplate));
        }

        public List<SeasonStatistics> SeriesStatistics(int seriesId)
        {
            var time = DateTime.UtcNow;

            return MapResults(Query(EpisodesBuilder(time).Where<Episode>(x => x.AuthorId == seriesId), _selectEpisodesTemplate),
                Query(EditionFilesBuilder().Where<EditionFile>(x => x.AuthorId == seriesId), _selectEditionFilesTemplate));
        }

        private List<SeasonStatistics> MapResults(List<SeasonStatistics> episodesResult, List<SeasonStatistics> filesResult)
        {
            episodesResult.ForEach(e =>
            {
                var file = filesResult.SingleOrDefault(f => f.AuthorId == e.AuthorId & f.BookNumber == e.BookNumber);

                e.SizeOnDisk = file?.SizeOnDisk ?? 0;
                e.ReleaseGroupsString = file?.ReleaseGroupsString;
            });

            return episodesResult;
        }

        private List<SeasonStatistics> Query(SqlBuilder builder, string template)
        {
            var sql = builder.AddTemplate(template).LogQuery();

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<SeasonStatistics>(sql.RawSql, sql.Parameters).ToList();
            }
        }

        private SqlBuilder EpisodesBuilder(DateTime currentDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("currentDate", currentDate, null);

            var trueIndicator = _database.DatabaseType == DatabaseType.PostgreSQL ? "true" : "1";
            var falseIndicator = _database.DatabaseType == DatabaseType.PostgreSQL ? "false" : "0";

            return new SqlBuilder(_database.DatabaseType)
            .Select($@"""Episodes"".""AuthorId"" AS AuthorId,
                             ""Episodes"".""BookNumber"",
                             COUNT(*) AS TotalEpisodeCount,
                             SUM(CASE WHEN ""AirDateUtc"" <= @currentDate OR ""EditionFileId"" > 0 THEN 1 ELSE 0 END) AS AvailableEpisodeCount,
                             SUM(CASE WHEN (""Monitored"" = {trueIndicator} AND ""AirDateUtc"" <= @currentDate) OR ""EditionFileId"" > 0 THEN 1 ELSE 0 END) AS EpisodeCount,
                             SUM(CASE WHEN ""EditionFileId"" > 0 THEN 1 ELSE 0 END) AS EditionFileCount,
                             MIN(CASE WHEN ""AirDateUtc"" < @currentDate OR ""Monitored"" = {falseIndicator} THEN NULL ELSE ""AirDateUtc"" END) AS NextAiringString,
                             MAX(CASE WHEN ""AirDateUtc"" >= @currentDate OR ""Monitored"" = {falseIndicator} THEN NULL ELSE ""AirDateUtc"" END) AS PreviousAiringString,
                             MAX(""AirDate"") AS LastAiredString",
                parameters)
            .GroupBy<Episode>(x => x.AuthorId)
            .GroupBy<Episode>(x => x.BookNumber);
        }

        private SqlBuilder EditionFilesBuilder()
        {
            if (_database.DatabaseType == DatabaseType.SQLite)
            {
                return new SqlBuilder(_database.DatabaseType)
                .Select(@"""AuthorId"",
                            ""BookNumber"",
                            SUM(COALESCE(""Size"", 0)) AS SizeOnDisk,
                            GROUP_CONCAT(""ReleaseGroup"", '|') AS ReleaseGroupsString")
                .GroupBy<EditionFile>(x => x.AuthorId)
                .GroupBy<EditionFile>(x => x.BookNumber);
            }

            return new SqlBuilder(_database.DatabaseType)
                .Select(@"""AuthorId"",
                            ""BookNumber"",
                            SUM(COALESCE(""Size"", 0)) AS SizeOnDisk,
                            string_agg(""ReleaseGroup"", '|') AS ReleaseGroupsString")
                .GroupBy<EditionFile>(x => x.AuthorId)
                .GroupBy<EditionFile>(x => x.BookNumber);
        }
    }
}
