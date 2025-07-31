using System.Collections.Generic;

namespace Readarr.Core.Tv
{
    // Stub interface for TV compatibility - to be removed
    public interface IEpisodeService
    {
        Episode GetEpisode(int id);
        List<Episode> GetEpisodes(IEnumerable<int> ids);
        List<Episode> GetEpisodesBySeries(int seriesId);
        List<Episode> GetEpisodesBySeason(int seriesId, int seasonNumber);
        List<Episode> GetEpisodesBySceneSeason(int seriesId, int seasonNumber);
        Episode FindEpisode(int seriesId, int seasonNumber, int episodeNumber);
        Episode FindEpisodeByTitle(int seriesId, int seasonNumber, string releaseTitle);
        List<Episode> GetEpisodesByFileId(int episodeFileId);
        void UpdateEpisode(Episode episode);
        void SetEpisodeMonitored(int episodeId, bool monitored);
        void SetEpisodeMonitoredBySeason(int seriesId, int seasonNumber, bool monitored);
        void UpdateEpisodes(List<Episode> episodes);
        void InsertMany(List<Episode> episodes);
        void UpdateMany(List<Episode> episodes);
        void DeleteMany(List<Episode> episodes);
        void SetEpisodeMonitoredBySeriesId(int seriesId, bool monitored);
    }
}