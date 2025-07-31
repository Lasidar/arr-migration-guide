using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Author
{
    public class AuthorResource : RestResource
    {
        public string AuthorName { get; set; }
        public string ForeignAuthorId { get; set; }
        public string TitleSlug { get; set; }
        public string Overview { get; set; }
        public string Disambiguation { get; set; }
        public List<Links> Links { get; set; }
        
        public DateTime? NextBook { get; set; }
        public DateTime? LastBook { get; set; }
        
        public List<MediaCover> Images { get; set; }
        public string RemotePoster { get; set; }
        
        public string Path { get; set; }
        public int QualityProfileId { get; set; }
        public int MetadataProfileId { get; set; }
        
        public bool Monitored { get; set; }
        public MonitoringOptions MonitoringOptions { get; set; }
        
        public string RootFolderPath { get; set; }
        public List<string> Genres { get; set; }
        public string CleanName { get; set; }
        public string SortName { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Added { get; set; }
        public AuthorStatisticsResource Statistics { get; set; }
        
        public bool? HasFile { get; set; }
        
        // Hiding this so people don't think its usable (only used to set the initial state)
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AddOptions { get; set; }
        
        // Hiding this so people don't think its usable (only used for searches)
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool SearchForMissingBooks { get; set; }
    }

    public static class AuthorResourceMapper
    {
        public static AuthorResource ToResource(this Core.Books.Author model)
        {
            if (model == null) return null;

            return new AuthorResource
            {
                Id = model.Id,
                AuthorName = model.Name,
                ForeignAuthorId = model.Metadata.Value.ForeignAuthorId,
                TitleSlug = model.Metadata.Value.TitleSlug,
                Overview = model.Metadata.Value.Overview,
                Disambiguation = model.Metadata.Value.Disambiguation,
                Links = model.Metadata.Value.Links,
                NextBook = model.Metadata.Value.NextBook,
                LastBook = model.Metadata.Value.LastBook,
                Images = model.Metadata.Value.Images,
                Path = model.Path,
                QualityProfileId = model.QualityProfileId,
                MetadataProfileId = model.MetadataProfileId,
                Monitored = model.Monitored,
                MonitoringOptions = model.MonitoringOptions,
                RootFolderPath = model.RootFolderPath,
                Genres = model.Metadata.Value.Genres,
                CleanName = model.CleanName,
                SortName = model.Metadata.Value.SortName,
                Tags = model.Tags,
                Added = model.Added,
                Statistics = new AuthorStatisticsResource(), // Will be populated separately
                HasFile = model.HasFile
            };
        }

        public static Core.Books.Author ToModel(this AuthorResource resource)
        {
            if (resource == null) return null;

            var author = new Core.Books.Author
            {
                Id = resource.Id,
                Name = resource.AuthorName,
                CleanName = resource.CleanName,
                Path = resource.Path,
                QualityProfileId = resource.QualityProfileId,
                MetadataProfileId = resource.MetadataProfileId,
                Monitored = resource.Monitored,
                MonitoringOptions = resource.MonitoringOptions,
                RootFolderPath = resource.RootFolderPath,
                Tags = resource.Tags,
                Added = resource.Added,
                AddOptions = new AddAuthorOptions
                {
                    Monitor = resource.MonitoringOptions.Monitor,
                    SearchForMissingBooks = resource.SearchForMissingBooks
                }
            };

            if (resource.ForeignAuthorId.IsNotNullOrWhiteSpace())
            {
                author.Metadata = new AuthorMetadata
                {
                    ForeignAuthorId = resource.ForeignAuthorId,
                    TitleSlug = resource.TitleSlug,
                    Name = resource.AuthorName,
                    Overview = resource.Overview,
                    Disambiguation = resource.Disambiguation,
                    Links = resource.Links,
                    Images = resource.Images,
                    Genres = resource.Genres,
                    SortName = resource.SortName
                };
            }

            return author;
        }

        public static Core.Books.Author ToModel(this AuthorResource resource, Core.Books.Author author)
        {
            var updatedAuthor = resource.ToModel();

            author.Path = updatedAuthor.Path;
            author.QualityProfileId = updatedAuthor.QualityProfileId;
            author.MetadataProfileId = updatedAuthor.MetadataProfileId;
            author.Monitored = updatedAuthor.Monitored;
            author.MonitoringOptions = updatedAuthor.MonitoringOptions;
            author.RootFolderPath = updatedAuthor.RootFolderPath;
            author.Tags = updatedAuthor.Tags;

            return author;
        }

        public static List<AuthorResource> ToResource(this IEnumerable<Core.Books.Author> models)
        {
            return models?.Select(ToResource).ToList();
        }
    }
}