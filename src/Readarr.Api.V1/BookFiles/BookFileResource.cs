using System;
using System.Collections.Generic;
using System.IO;
using Readarr.Core.CustomFormats;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Api.V3.CustomFormats;
using Readarr.Http.REST;

namespace Readarr.Api.V1.BookFiles
{
    public class BookFileResource : RestResource
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public int EditionId { get; set; }
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
        
        // Book-specific
        public string CalibreId { get; set; }
        public int Part { get; set; }

        public bool QualityCutoffNotMet { get; set; }
    }

    public static class BookFileResourceMapper
    {
        public static BookFileResource ToResource(this BookFile model, Core.Books.Author author, IUpgradableSpecification upgradableSpecification, ICustomFormatCalculationService formatCalculationService)
        {
            if (model == null)
            {
                return null;
            }

            model.Author = author;
            var customFormats = formatCalculationService?.ParseCustomFormat(model, author);
            var customFormatScore = author?.QualityProfile?.Value?.CalculateCustomFormatScore(customFormats) ?? 0;

            return new BookFileResource
            {
                Id = model.Id,

                AuthorId = model.AuthorId,
                BookId = model.BookId,
                EditionId = model.EditionId,
                RelativePath = model.RelativePath,
                Path = Path.Combine(author.Path, model.RelativePath),
                Size = model.Size,
                DateAdded = model.DateAdded,
                SceneName = model.SceneName,
                ReleaseGroup = model.ReleaseGroup,
                Languages = model.Languages,
                Quality = model.Quality,
                MediaInfo = model.MediaInfo.ToResource(model.SceneName),
                QualityCutoffNotMet = upgradableSpecification.QualityCutoffNotMet(author.QualityProfile.Value, model.Quality),
                CustomFormats = customFormats.ToResource(false),
                CustomFormatScore = customFormatScore,
                IndexerFlags = (int)model.IndexerFlags,
                ReleaseType = model.ReleaseType,
                CalibreId = model.CalibreId,
                Part = model.Part
            };
        }
    }
}