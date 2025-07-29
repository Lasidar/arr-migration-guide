using System.Collections.Generic;

namespace Readarr.Core.Books
{
    public interface ISeriesService
    {
        Series GetSeries(int seriesId);
        List<Series> GetAllSeries();
        List<Series> GetByAuthorId(int authorId);
        Series AddSeries(Series series);
        Series UpdateSeries(Series series);
        void DeleteSeries(int seriesId);
        
        // Book linking
        void LinkBookToSeries(int seriesId, int bookId, string position);
        void UnlinkBookFromSeries(int seriesId, int bookId);
        List<SeriesBookLink> GetSeriesBookLinks(int seriesId);
        
        // Search methods
        Series FindByForeignSeriesId(string foreignSeriesId);
        Series FindByTitle(string title);
        List<Series> FindByTitleInexact(string title);
        Dictionary<int, List<int>> GetAllSeriesBookIds();
    }
}