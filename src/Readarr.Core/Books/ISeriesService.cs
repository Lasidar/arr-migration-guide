using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.Books
{
    public interface ISeriesService
    {
        Series GetSeries(int seriesId);
        List<Tv.Series> GetAllSeries();
        List<Tv.Series> GetByAuthorId(int authorId);
        Series AddSeries(Tv.Series series);
        Series UpdateSeries(Tv.Series series);
        void DeleteSeries(int seriesId);
        
        // Book linking
        void LinkBookToSeries(int seriesId, int bookId, string position);
        void UnlinkBookFromSeries(int seriesId, int bookId);
        List<SeriesBookLink> GetSeriesBookLinks(int seriesId);
        
        // Search methods
        Series FindByForeignSeriesId(string foreignSeriesId);
        Series FindByTitle(string title);
        List<Tv.Series> FindByTitleInexact(string title);
        Dictionary<int, List<int>> GetAllSeriesBookIds();
    }
}