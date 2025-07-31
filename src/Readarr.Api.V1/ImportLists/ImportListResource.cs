using System;
using Readarr.Core.ImportLists;
using Readarr.Core.Books;

namespace Readarr.Api.V1.ImportLists
{
    public class ImportListResource : ProviderResource<ImportListResource>
    {
        public bool EnableAutomaticAdd { get; set; }
        public bool SearchForMissingBooks { get; set; }
        public MonitorTypes ShouldMonitor { get; set; }
        public NewItemMonitorTypes MonitorNewItems { get; set; }
        public string RootFolderPath { get; set; }
        public int QualityProfileId { get; set; }
        public SeriesTypes SeriesType { get; set; }
        public bool SeasonFolder { get; set; }
        public ImportListType ListType { get; set; }
        public int ListOrder { get; set; }
        public TimeSpan MinRefreshInterval { get; set; }
    }

    public class ImportListResourceMapper : ProviderResourceMapper<ImportListResource, ImportListDefinition>
    {
        public override ImportListResource ToResource(ImportListDefinition definition)
        {
            if (definition == null)
            {
                return null;
            }

            var resource = base.ToResource(definition);

            resource.EnableAutomaticAdd = definition.EnableAutomaticAdd;
            resource.SearchForMissingBooks = definition.SearchForMissingBooks;
            resource.ShouldMonitor = definition.ShouldMonitor;
            resource.MonitorNewItems = definition.MonitorNewItems;
            resource.RootFolderPath = definition.RootFolderPath;
            resource.QualityProfileId = definition.QualityProfileId;
            resource.SeriesType = definition.SeriesType;
            resource.SeasonFolder = definition.SeasonFolder;
            resource.ListType = definition.ListType;
            resource.ListOrder = (int)definition.ListType;
            resource.MinRefreshInterval = definition.MinRefreshInterval;

            return resource;
        }

        public override ImportListDefinition ToModel(ImportListResource resource, ImportListDefinition existingDefinition)
        {
            if (resource == null)
            {
                return null;
            }

            var definition = base.ToModel(resource, existingDefinition);

            definition.EnableAutomaticAdd = resource.EnableAutomaticAdd;
            definition.SearchForMissingBooks = resource.SearchForMissingBooks;
            definition.ShouldMonitor = resource.ShouldMonitor;
            definition.MonitorNewItems = resource.MonitorNewItems;
            definition.RootFolderPath = resource.RootFolderPath;
            definition.QualityProfileId = resource.QualityProfileId;
            definition.SeriesType = resource.SeriesType;
            definition.SeasonFolder = resource.SeasonFolder;
            definition.ListType = resource.ListType;
            definition.MinRefreshInterval = resource.MinRefreshInterval;

            return definition;
        }
    }
}
