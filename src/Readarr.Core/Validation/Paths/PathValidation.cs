using System;
using System.IO;
using System.Linq;
using Readarr.Core.RootFolders;

namespace Readarr.Core.Validation.Paths
{
    public interface IPathValidator
    {
        bool IsValidPath(string path);
        bool IsPathAccessible(string path);
    }

    public class PathValidator : IPathValidator
    {
        private readonly IRootFolderService _rootFolderService;

        public PathValidator(IRootFolderService rootFolderService)
        {
            _rootFolderService = rootFolderService;
        }

        public bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                // Normalize the path to prevent traversal attacks
                var normalizedPath = Path.GetFullPath(path);
                
                // Get all allowed root folders
                var allowedPaths = _rootFolderService.GetAll()
                    .Select(rf => Path.GetFullPath(rf.Path))
                    .ToList();

                // Check if the path is within an allowed root folder
                return allowedPaths.Any(allowed => 
                    normalizedPath.StartsWith(allowed, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                // Invalid path format
                return false;
            }
        }

        public bool IsPathAccessible(string path)
        {
            if (!IsValidPath(path))
                return false;

            try
            {
                // Check if we can access the directory
                var dirInfo = new DirectoryInfo(path);
                return dirInfo.Exists || dirInfo.Parent?.Exists == true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}