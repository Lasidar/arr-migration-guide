using System.Collections.Generic;

namespace Readarr.Core.DataAugmentation.Scene
{
    public interface ISceneMappingProvider
    {
        List<SceneMapping> GetSceneMappings();
    }
}
