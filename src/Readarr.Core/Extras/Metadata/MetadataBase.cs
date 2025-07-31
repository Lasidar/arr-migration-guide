using System;
using System.Collections.Generic;
using System.IO;
using FluentValidation.Results;
using Readarr.Core.Extras.Metadata.Files;
using Readarr.Core.MediaFiles;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Extras.Metadata
{
    public abstract class MetadataBase<TSettings> : IMetadata
        where TSettings : IProviderConfig, new()
    {
        public abstract string Name { get; }

        public Type ConfigContract => typeof(TSettings);

        public virtual ProviderMessage Message => null;

        public IEnumerable<ProviderDefinition> DefaultDefinitions => new List<ProviderDefinition>();

        public ProviderDefinition Definition { get; set; }

        public ValidationResult Test()
        {
            return new ValidationResult();
        }

        public virtual string GetFilenameAfterMove(Tv.Series series, EpisodeFile episodeFile, MetadataFile metadataFile)
        {
            var existingFilename = Path.Combine(series.Path, metadataFile.RelativePath);
            var extension = Path.GetExtension(existingFilename).TrimStart('.');
            var newFileName = Path.ChangeExtension(Path.Combine(series.Path, episodeFile.RelativePath), extension);

            return newFileName;
        }

        public abstract MetadataFile FindMetadataFile(Tv.Series series, string path);

        public abstract MetadataFileResult SeriesMetadata(Tv.Series series, SeriesMetadataReason reason);
        public abstract MetadataFileResult EpisodeMetadata(Tv.Series series, EpisodeFile episodeFile);
        public abstract List<ImageFileResult> SeriesImages(Tv.Series series);
        public abstract List<ImageFileResult> SeasonImages(Tv.Series series, Season season);
        public abstract List<ImageFileResult> EpisodeImages(Tv.Series series, EpisodeFile episodeFile);

        public virtual object RequestAction(string action, IDictionary<string, string> query)
        {
            return null;
        }

        protected TSettings Settings => (TSettings)Definition.Settings;

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
