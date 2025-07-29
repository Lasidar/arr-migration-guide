using System;
using System.Collections.Generic;
using System.IO;
using NzbDrone.Core.CustomFormats;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Qualities;
using Readarr.Api.V3.CustomFormats;
using Readarr.Http.REST;

namespace Readarr.Api.V3.EditionFiles
{
    public class EditionFileResource : RestResource
    {
        public int AuthorId { get; set; }
        public int BookNumber { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
        public string SceneName { get; set; }
        public string ReleaseGroup { get; set; }
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public List<CustomFormatResource> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
        public int? IndexerFlags { get; set; }
        public ReleaseType? ReleaseType { get; set; }
        public MediaInfoResource MediaInfo { get; set; }

        public bool QualityCutoffNotMet { get; set; }
    }

    public static class EditionFileResourceMapper
    {
        public static EditionFileResource ToResource(this EditionFile model, NzbDrone.Core.Tv.Series series, IUpgradableSpecification upgradableSpecification, ICustomFormatCalculationService formatCalculationService)
        {
            if (model == null)
            {
                return null;
            }

            model.Series = series;
            var customFormats = formatCalculationService?.ParseCustomFormat(model, model.Series);
            var customFormatScore = series?.QualityProfile?.Value?.CalculateCustomFormatScore(customFormats) ?? 0;

            return new EditionFileResource
            {
                Id = model.Id,

                AuthorId = model.AuthorId,
                BookNumber = model.BookNumber,
                RelativePath = model.RelativePath,
                Path = Path.Combine(series.Path, model.RelativePath),
                Size = model.Size,
                DateAdded = model.DateAdded,
                SceneName = model.SceneName,
                ReleaseGroup = model.ReleaseGroup,
                Languages = model.Languages,
                Quality = model.Quality,
                MediaInfo = model.MediaInfo.ToResource(model.SceneName),
                QualityCutoffNotMet = upgradableSpecification.QualityCutoffNotMet(series.QualityProfile.Value, model.Quality),
                CustomFormats = customFormats.ToResource(false),
                CustomFormatScore = customFormatScore,
                IndexerFlags = (int)model.IndexerFlags,
                ReleaseType = model.ReleaseType,
            };
        }
    }
}
