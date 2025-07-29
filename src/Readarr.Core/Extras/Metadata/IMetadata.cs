using System.Collections.Generic;
using Readarr.Core.Extras.Metadata.Files;
using Readarr.Core.MediaFiles;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Tv;

namespace Readarr.Core.Extras.Metadata
{
    public interface IMetadata : IProvider
    {
        string GetFilenameAfterMove(Series series, EpisodeFile episodeFile, MetadataFile metadataFile);
        MetadataFile FindMetadataFile(Series series, string path);
        MetadataFileResult SeriesMetadata(Series series, SeriesMetadataReason reason);
        MetadataFileResult EpisodeMetadata(Series series, EpisodeFile episodeFile);
        List<ImageFileResult> SeriesImages(Series series);
        List<ImageFileResult> SeasonImages(Series series, Season season);
        List<ImageFileResult> EpisodeImages(Series series, EpisodeFile episodeFile);
    }

    public enum SeriesMetadataReason
    {
        Scan,
        EpisodeFolderCreated,
        EpisodesImported
    }
}
