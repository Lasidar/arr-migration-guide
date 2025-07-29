using System.Collections.Generic;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists
{
    public interface IParseImportListResponse
    {
        IList<ImportListItemInfo> ParseResponse(ImportListResponse importListResponse);
    }
}
