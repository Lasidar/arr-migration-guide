using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles
{
    // Stub interface for TV compatibility - to be removed
    public interface IMediaFileTableCleanupService
    {
        void Clean(Tv.Series series, List<string> filesOnDisk);
    }
}