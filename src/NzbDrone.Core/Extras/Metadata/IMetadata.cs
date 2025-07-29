using System.Collections.Generic;
using NzbDrone.Core.Extras.Metadata.Files;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Extras.Metadata
{
    public interface IMetadata : IProvider
    {
        string GetFilenameAfterMove(Series series, EditionFile episodeFile, MetadataFile metadataFile);
        MetadataFile FindMetadataFile(Series series, string path);
        MetadataFileResult SeriesMetadata(Series series, SeriesMetadataReason reason);
        MetadataFileResult EpisodeMetadata(Series series, EditionFile episodeFile);
        List<ImageFileResult> SeriesImages(Series series);
        List<ImageFileResult> SeasonImages(Series series, Season season);
        List<ImageFileResult> EpisodeImages(Series series, EditionFile episodeFile);
    }

    public enum SeriesMetadataReason
    {
        Scan,
        EpisodeFolderCreated,
        EpisodesImported
    }
}
