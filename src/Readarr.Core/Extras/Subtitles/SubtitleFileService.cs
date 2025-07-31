using NLog;
using Readarr.Common.Disk;
using Readarr.Core.Extras.Files;
using Readarr.Core.MediaFiles;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Extras.Subtitles
{
    public interface ISubtitleFileService : IExtraFileService<SubtitleFile>
    {
    }

    public class SubtitleFileService : ExtraFileService<SubtitleFile>, ISubtitleFileService
    {
        public SubtitleFileService(IExtraFileRepository<SubtitleFile> repository, Tv.ISeriesService seriesService, IDiskProvider diskProvider, IRecycleBinProvider recycleBinProvider, Logger logger)
            : base(repository, seriesService, diskProvider, recycleBinProvider, logger)
        {
        }
    }
}
