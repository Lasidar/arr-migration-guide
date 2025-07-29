using Readarr.Core.DecisionEngine;

namespace Readarr.Core.MediaFiles.BookImport.Specifications
{
    public interface IImportDecisionEngineSpecification<T>
    {
        Decision IsSatisfiedBy(T item);
    }
}