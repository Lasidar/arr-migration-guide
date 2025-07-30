using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    // Interface that supports both TV and book specifications
    // This allows specifications to be gradually migrated from TV to books
    public interface IDualDownloadDecisionEngineSpecification : IDownloadDecisionEngineSpecification, IBookDownloadDecisionEngineSpecification
    {
        // Inherits both TV and book methods
    }
}