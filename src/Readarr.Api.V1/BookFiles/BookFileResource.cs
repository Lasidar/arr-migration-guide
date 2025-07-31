using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Books;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.MediaInfo;
using Readarr.Core.Qualities;
using Readarr.Http.REST;

namespace Readarr.Api.V1.BookFiles
{
    public class BookFileResource : RestResource
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
        public string SceneName { get; set; }
        public string ReleaseGroup { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public MediaInfoResource MediaInfo { get; set; }
        public bool QualityCutoffNotMet { get; set; }
        public List<CustomFormatResource> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }
    }

    public static class BookFileResourceMapper
    {
        private static BookFileResource ToResource(this BookFile model)
        {
            if (model == null) return null;

            return new BookFileResource
            {
                Id = model.Id,
                BookId = model.BookId,
                Path = model.Path,
                Size = model.Size,
                DateAdded = model.DateAdded,
                SceneName = model.SceneName,
                ReleaseGroup = model.ReleaseGroup,
                Quality = model.Quality,
                Languages = model.Languages,
                MediaInfo = model.MediaInfo?.ToResource() ?? new MediaInfoResource(),
                IndexerFlags = model.IndexerFlags,
                ReleaseType = model.ReleaseType
            };
        }

        public static BookFileResource ToResource(this BookFile model, Author author, Book book, IUpgradableSpecification upgradableSpecification)
        {
            if (model == null) return null;

            var resource = model.ToResource();
            
            if (author != null)
            {
                resource.AuthorId = author.Id;
            }

            if (book != null && upgradableSpecification != null)
            {
                resource.QualityCutoffNotMet = upgradableSpecification.QualityCutoffNotMet(author.QualityProfile.Value, model.Quality);
            }

            return resource;
        }

        public static List<BookFileResource> ToResource(this IEnumerable<BookFile> models, Author author, Book book, IUpgradableSpecification upgradableSpecification)
        {
            return models.Select(model => model.ToResource(author, book, upgradableSpecification)).ToList();
        }
    }

    public class MediaInfoResource
    {
        public decimal AudioChannels { get; set; }
        public string AudioCodec { get; set; }
        public List<string> AudioLanguages { get; set; }
        public int AudioBitrate { get; set; }
        public int AudioSampleRate { get; set; }
        public TimeSpan Duration { get; set; }
        public string Format { get; set; }
    }

    public static class MediaInfoResourceMapper
    {
        public static MediaInfoResource ToResource(this MediaInfoModel model)
        {
            if (model == null) return null;

            return new MediaInfoResource
            {
                AudioChannels = MediaInfoFormatter.FormatAudioChannels(model),
                AudioCodec = MediaInfoFormatter.FormatAudioCodec(model, null),
                AudioLanguages = model.AudioLanguages?.Distinct().ToList() ?? new List<string>(),
                AudioBitrate = model.AudioBitrate,
                AudioSampleRate = model.AudioSampleRate,
                Duration = model.RunTime,
                Format = model.ContainerFormat
            };
        }
    }

    public class CustomFormatResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}