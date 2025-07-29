using System.Collections.Generic;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Download.Pending
{
    public interface IPendingReleaseRepository : IBasicRepository<PendingRelease>
    {
        void DeleteByAuthorIds(List<int> seriesIds);
        List<PendingRelease> AllByAuthorId(int seriesId);
        List<PendingRelease> WithoutFallback();
    }

    public class PendingReleaseRepository : BasicRepository<PendingRelease>, IPendingReleaseRepository
    {
        public PendingReleaseRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public void DeleteByAuthorIds(List<int> seriesIds)
        {
            Delete(r => seriesIds.Contains(r.AuthorId));
        }

        public List<PendingRelease> AllByAuthorId(int seriesId)
        {
            return Query(p => p.AuthorId == seriesId);
        }

        public List<PendingRelease> WithoutFallback()
        {
            var builder = new SqlBuilder(_database.DatabaseType)
                .InnerJoin<PendingRelease, Series>((p, s) => p.AuthorId == s.Id)
                .Where<PendingRelease>(p => p.Reason != PendingReleaseReason.Fallback);

            return Query(builder);
        }
    }
}
