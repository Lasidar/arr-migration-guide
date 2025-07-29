using System.Collections.Generic;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.DataAugmentation.Scene
{
    public interface ISceneMappingRepository : IBasicRepository<SceneMapping>
    {
        List<SceneMapping> FindByTvdbid(int tvdbId);
        void Clear(string type);
    }

    public class SceneMappingRepository : BasicRepository<SceneMapping>, ISceneMappingRepository
    {
        public SceneMappingRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public List<SceneMapping> FindByTvdbid(int tvdbId)
        {
            return Query(x => x.TvdbId == tvdbId);
        }

        public void Clear(string type)
        {
            Delete(s => s.Type == type);
        }
    }
}
