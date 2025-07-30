using System;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.ImportLists
{
    public interface IBookImportList : IProvider
    {
        ImportListType ListType { get; }
        TimeSpan MinRefreshInterval { get; }
        ImportListBookFetchResult Fetch();
    }
}