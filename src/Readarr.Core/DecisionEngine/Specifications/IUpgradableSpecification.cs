namespace Readarr.Core.DecisionEngine.Specifications
{
    public interface IUpgradableSpecification
    {
        bool IsUpgradable(object current, object newItem);
    }
}