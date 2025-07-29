using Readarr.Core.Datastore;

namespace Readarr.Core.ThingiProvider
{
    public interface IProviderRepository<TProvider> : IBasicRepository<TProvider>
        where TProvider : ModelBase, new()
    {
// void DeleteImplementations(string implementation);
    }
}
