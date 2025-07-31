using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.DecisionEngine;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles.BookImport.Manual;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Http.REST;

namespace Readarr.Api.V1.ManualImport
{
    public class ManualImportResource : RestResource
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public Author Author { get; set; }
        public Book Book { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public int QualityWeight { get; set; }
        public string DownloadId { get; set; }
        public List<Rejection> Rejections { get; set; }
        public ParsedBookInfo ParsedBookInfo { get; set; }
        public bool AdditionalFile { get; set; }
        public bool ReplaceExistingFiles { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }

        public class Author
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ForeignAuthorId { get; set; }
        }

        public class Book
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string ForeignBookId { get; set; }
        }
    }

    public static class ManualImportResourceMapper
    {
        public static ManualImportResource ToResource(this ManualImportItem model)
        {
            if (model == null)
            {
                return null;
            }

            var resource = new ManualImportResource
            {
                Id = model.Id,
                Path = model.Path,
                Name = model.Name,
                Size = model.Size,
                Quality = model.Quality,
                Languages = model.Languages,
                ReleaseGroup = model.ReleaseGroup,
                DownloadId = model.DownloadId,
                Rejections = model.Rejections,
                ParsedBookInfo = model.ParsedBookInfo,
                AdditionalFile = model.AdditionalFile,
                ReplaceExistingFiles = model.ReplaceExistingFiles,
                IndexerFlags = model.IndexerFlags,
                ReleaseType = model.ReleaseType
            };

            if (model.Author != null)
            {
                resource.Author = new ManualImportResource.Author
                {
                    Id = model.Author.Id,
                    Name = model.Author.Name,
                    ForeignAuthorId = model.Author.Metadata.Value?.ForeignAuthorId
                };
            }

            if (model.Book != null)
            {
                resource.Book = new ManualImportResource.Book
                {
                    Id = model.Book.Id,
                    Title = model.Book.Title,
                    ForeignBookId = model.Book.ForeignBookId
                };
            }

            if (model.Quality != null)
            {
                resource.QualityWeight = Core.Qualities.Quality.DefaultQualityDefinitions
                    .Single(q => q.Quality == model.Quality.Quality).Weight;
                resource.QualityWeight += model.Quality.Revision.Real * 10;
                resource.QualityWeight += model.Quality.Revision.Version;
            }

            return resource;
        }

        public static List<ManualImportResource> ToResource(this IEnumerable<ManualImportItem> models)
        {
            return models.Select(ToResource).ToList();
        }
    }
}
