using System.Collections.Generic;
using Readarr.Core.Extras.Metadata.Files;
using Readarr.Core.MediaFiles;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Extras.Metadata
{
    public interface IMetadata : IProvider
    {
        string GetFilenameAfterMove(Tv.Series series, EpisodeFile episodeFile, MetadataFile metadataFile);
        MetadataFile FindMetadataFile(Tv.Series series, string path);
        MetadataFileResult SeriesMetadata(Tv.Series series, SeriesMetadataReason reason);
        MetadataFileResult EpisodeMetadata(Tv.Series series, EpisodeFile episodeFile);
        List<ImageFileResult> SeriesImages(Tv.Series series);
        List<ImageFileResult> SeasonImages(Tv.Series series, Season season);
        List<ImageFileResult> EpisodeImages(Tv.Series series, EpisodeFile episodeFile);
    }

    public enum SeriesMetadataReason
    {
        Scan,
        EpisodeFolderCreated,
        EpisodesImported
    }
}
