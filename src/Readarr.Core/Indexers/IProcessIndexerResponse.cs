using System.Collections.Generic;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Indexers
{
    public interface IParseIndexerResponse
    {
        IList<ReleaseInfo> ParseResponse(IndexerResponse indexerResponse);
    }
}
