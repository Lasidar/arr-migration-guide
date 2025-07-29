using Readarr.Core.Books;
using Readarr.Core.MediaFiles;
using Readarr.Core.Tv;
using System.Collections.Generic;

namespace Readarr.Core.Organizer
{
    public interface IBuildFileNames
    {
        // Book methods
        string GetAuthorFolder(Author author);
        string GetBookFolder(Book book);
        string GetBookFileName(Book book, BookFile bookFile);
        
        // TV methods (for compatibility)
        string GetSeriesFolder(Series series);
        string GetSeasonFolder(Series series, int seasonNumber);
        string BuildFileName(List<Episode> episodes, Series series, EpisodeFile episodeFile, string extension = "", NamingConfig namingConfig = null, List<string> preferredWords = null);
        string BuildFilePath(List<Episode> episodes, Series series, EpisodeFile episodeFile, string extension = "", NamingConfig namingConfig = null, List<string> preferredWords = null);
        string BuildSeasonPath(Series series, int seasonNumber);
        bool RequiresEpisodeTitle(Series series, List<Episode> episodes);
        bool RequiresAbsoluteEpisodeNumber(Series series, List<Episode> episodes);
    }
}