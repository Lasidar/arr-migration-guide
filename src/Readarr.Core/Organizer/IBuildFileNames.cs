using Readarr.Core.Books;
using Readarr.Core.MediaFiles;
using Readarr.Core.Books;
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
        string GetSeriesFolder(Tv.Series series);
        string GetSeasonFolder(Tv.Series series, int seasonNumber);
        string BuildFileName(List<Episode> episodes, Tv.Series series, EpisodeFile episodeFile, string extension = "", NamingConfig namingConfig = null, List<string> preferredWords = null);
        string BuildFilePath(List<Episode> episodes, Tv.Series series, EpisodeFile episodeFile, string extension = "", NamingConfig namingConfig = null, List<string> preferredWords = null);
        string BuildSeasonPath(Tv.Series series, int seasonNumber);
        bool RequiresEpisodeTitle(Tv.Series series, List<Episode> episodes);
        bool RequiresAbsoluteEpisodeNumber(Tv.Series series, List<Episode> episodes);
    }
}