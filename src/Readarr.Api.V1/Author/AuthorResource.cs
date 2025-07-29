using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Author
{
    public class AuthorResource : RestResource
    {
        // View Only
        public string Name { get; set; }
        public string SortName { get; set; }
        public AuthorStatusType Status { get; set; }
        
        public string Overview { get; set; }
        public List<MediaCover> Images { get; set; }
        public string RemotePoster { get; set; }
        
        // Author specific
        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }
        public string Gender { get; set; }
        public string Hometown { get; set; }
        public string Website { get; set; }
        
        // View & Edit
        public string Path { get; set; }
        public int QualityProfileId { get; set; }
        public int MetadataProfileId { get; set; }
        
        // Editing Only
        public bool Monitored { get; set; }
        public NewItemMonitorTypes MonitorNewItems { get; set; }
        
        // IDs
        public string ForeignAuthorId { get; set; }
        public string CleanName { get; set; }
        public string TitleSlug { get; set; }
        public string RootFolderPath { get; set; }
        public string Folder { get; set; }
        public List<string> Genres { get; set; }
        public HashSet<int> Tags { get; set; }
        public DateTime Added { get; set; }
        public AddAuthorOptions AddOptions { get; set; }
        public Ratings Ratings { get; set; }
        
        public AuthorStatisticsResource Statistics { get; set; }
        
        // Metadata
        public AuthorMetadataResource Metadata { get; set; }
    }

    public class AuthorMetadataResource : RestResource
    {
        public string ForeignAuthorId { get; set; }
        public string GoodreadsId { get; set; }
        public string IsniId { get; set; }
        public string AsinId { get; set; }
        public string Name { get; set; }
        public string SortName { get; set; }
        public string NameSlug { get; set; }
        public string Overview { get; set; }
        public string Gender { get; set; }
        public string Hometown { get; set; }
        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }
        public string Website { get; set; }
        public AuthorStatusType Status { get; set; }
        public List<MediaCover> Images { get; set; }
        public List<string> Genres { get; set; }
        public List<Links> Links { get; set; }
        public List<string> Aliases { get; set; }
        public Ratings Ratings { get; set; }
    }

    public static class AuthorResourceMapper
    {
        public static AuthorResource ToResource(this Core.Books.Author model)
        {
            if (model == null)
            {
                return null;
            }

            return new AuthorResource
            {
                Id = model.Id,
                
                // Basic info
                Name = model.Metadata.Value?.Name ?? "Unknown",
                SortName = model.SortName,
                Status = model.Metadata.Value?.Status ?? AuthorStatusType.Continuing,
                Overview = model.Metadata.Value?.Overview,
                
                // Author specific
                Born = model.Metadata.Value?.Born,
                Died = model.Metadata.Value?.Died,
                Gender = model.Metadata.Value?.Gender,
                Hometown = model.Metadata.Value?.Hometown,
                Website = model.Metadata.Value?.Website,
                
                // IDs
                ForeignAuthorId = model.Metadata.Value?.ForeignAuthorId,
                CleanName = model.CleanName,
                
                // Paths
                Path = model.Path,
                RootFolderPath = model.RootFolderPath,
                
                // Monitoring
                Monitored = model.Monitored,
                MonitorNewItems = model.MonitorNewItems,
                
                // Profiles
                QualityProfileId = model.QualityProfileId,
                MetadataProfileId = model.MetadataProfileId,
                
                // Metadata
                Images = model.Metadata.Value?.Images ?? new List<MediaCover>(),
                Genres = model.Metadata.Value?.Genres ?? new List<string>(),
                Ratings = model.Metadata.Value?.Ratings,
                
                // System
                Tags = model.Tags,
                Added = model.Added,
                AddOptions = model.AddOptions,
                
                // Metadata object
                Metadata = model.Metadata.Value?.ToResource()
            };
        }

        public static Core.Books.Author ToModel(this AuthorResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new Core.Books.Author
            {
                Id = resource.Id,
                
                // Names
                CleanName = resource.CleanName,
                SortName = resource.SortName,
                
                // Paths
                Path = resource.Path,
                RootFolderPath = resource.RootFolderPath,
                
                // Monitoring
                Monitored = resource.Monitored,
                MonitorNewItems = resource.MonitorNewItems,
                
                // Profiles
                QualityProfileId = resource.QualityProfileId,
                MetadataProfileId = resource.MetadataProfileId,
                
                // System
                Tags = resource.Tags,
                Added = resource.Added,
                AddOptions = resource.AddOptions
            };
        }

        public static AuthorResource ToResource(this Core.Books.Author model, AuthorStatisticsResource statistics)
        {
            var resource = model.ToResource();
            resource.Statistics = statistics;
            return resource;
        }

        public static List<AuthorResource> ToResource(this IEnumerable<Core.Books.Author> models)
        {
            return models?.Select(ToResource).ToList();
        }

        public static AuthorMetadataResource ToResource(this AuthorMetadata model)
        {
            if (model == null)
            {
                return null;
            }

            return new AuthorMetadataResource
            {
                Id = model.Id,
                ForeignAuthorId = model.ForeignAuthorId,
                GoodreadsId = model.GoodreadsId,
                IsniId = model.IsniId,
                AsinId = model.AsinId,
                Name = model.Name,
                SortName = model.SortName,
                NameSlug = model.NameSlug,
                Overview = model.Overview,
                Gender = model.Gender,
                Hometown = model.Hometown,
                Born = model.Born,
                Died = model.Died,
                Website = model.Website,
                Status = model.Status,
                Images = model.Images,
                Genres = model.Genres,
                Links = model.Links,
                Aliases = model.Aliases,
                Ratings = model.Ratings
            };
        }
    }
}