using Readarr.Core.Datastore;
using Readarr.Core.Extras.Files;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Extras.Subtitles
{
    public interface ISubtitleFileRepository : IExtraFileRepository<SubtitleFile>
    {
    }

    public class SubtitleFileRepository : ExtraFileRepository<SubtitleFile>, ISubtitleFileRepository
    {
        public SubtitleFileRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
