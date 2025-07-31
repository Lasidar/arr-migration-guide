using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles
{
    // Stub interface for TV compatibility - to be removed
    public interface IMoveEpisodeFiles
    {
        EpisodeFile MoveEpisodeFile(EpisodeFile episodeFile, Tv.Series series);
        EpisodeFile MoveEpisodeFile(EpisodeFile episodeFile, LocalEpisode localEpisode);
        EpisodeFile CopyEpisodeFile(EpisodeFile episodeFile, LocalEpisode localEpisode);
    }
}