using NLog;
using Readarr.Common.Disk;
using Readarr.Core.Extras.Files;
using Readarr.Core.MediaFiles;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Extras.Others
{
    public interface IOtherExtraFileService : IExtraFileService<OtherExtraFile>
    {
    }

    public class OtherExtraFileService : ExtraFileService<OtherExtraFile>, IOtherExtraFileService
    {
        public OtherExtraFileService(IExtraFileRepository<OtherExtraFile> repository, Tv.ISeriesService seriesService, IDiskProvider diskProvider, IRecycleBinProvider recycleBinProvider, Logger logger)
            : base(repository, seriesService, diskProvider, recycleBinProvider, logger)
        {
        }
    }
}
