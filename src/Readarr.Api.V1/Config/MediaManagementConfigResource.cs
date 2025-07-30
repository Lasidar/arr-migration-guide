using Readarr.Core.Configuration;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.EpisodeImport;
using Readarr.Core.Qualities;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Config
{
    public class MediaManagementConfigResource : RestResource
    {
        public bool AutoUnmonitorPreviouslyDownloadedBooks { get; set; }
        public string RecycleBin { get; set; }
        public int RecycleBinCleanupDays { get; set; }
        public ProperDownloadTypes DownloadPropersAndRepacks { get; set; }
        public bool CreateEmptySeriesFolders { get; set; }
        public bool DeleteEmptyFolders { get; set; }
        public FileDateType FileDate { get; set; }
        public RescanAfterRefreshType RescanAfterRefresh { get; set; }

        public bool SetPermissionsLinux { get; set; }
        public string ChmodFolder { get; set; }
        public string ChownGroup { get; set; }

        public EpisodeTitleRequiredType EpisodeTitleRequired { get; set; }
        public bool SkipFreeSpaceCheckWhenImporting { get; set; }
        public int MinimumFreeSpaceWhenImporting { get; set; }
        public bool CopyUsingHardlinks { get; set; }
        public bool UseScriptImport { get; set; }
        public string ScriptImportPath { get; set; }
        public bool ImportExtraFiles { get; set; }
        public string ExtraFileExtensions { get; set; }
        public bool EnableMediaInfo { get; set; }
        public string UserRejectedExtensions { get; set; }
    }

    public static class MediaManagementConfigResourceMapper
    {
        public static MediaManagementConfigResource ToResource(IConfigService model)
        {
            return new MediaManagementConfigResource
            {
                AutoUnmonitorPreviouslyDownloadedBooks = model.AutoUnmonitorPreviouslyDownloadedBooks,
                RecycleBin = model.RecycleBin,
                RecycleBinCleanupDays = model.RecycleBinCleanupDays,
                DownloadPropersAndRepacks = model.DownloadPropersAndRepacks,
                CreateEmptySeriesFolders = model.CreateEmptySeriesFolders,
                DeleteEmptyFolders = model.DeleteEmptyFolders,
                FileDate = model.FileDate,
                RescanAfterRefresh = model.RescanAfterRefresh,

                SetPermissionsLinux = model.SetPermissionsLinux,
                ChmodFolder = model.ChmodFolder,
                ChownGroup = model.ChownGroup,

                EpisodeTitleRequired = model.EpisodeTitleRequired,
                SkipFreeSpaceCheckWhenImporting = model.SkipFreeSpaceCheckWhenImporting,
                MinimumFreeSpaceWhenImporting = model.MinimumFreeSpaceWhenImporting,
                CopyUsingHardlinks = model.CopyUsingHardlinks,
                UseScriptImport = model.UseScriptImport,
                ScriptImportPath = model.ScriptImportPath,
                ImportExtraFiles = model.ImportExtraFiles,
                ExtraFileExtensions = model.ExtraFileExtensions,
                EnableMediaInfo = model.EnableMediaInfo,
                UserRejectedExtensions = model.UserRejectedExtensions
            };
        }
    }
}
