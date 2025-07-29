using System;
using System.Collections.Generic;
using Readarr.Core.Indexers;
using Readarr.Core.Validation;

namespace Readarr.Core.Test.IndexerTests
{
    public class TestIndexerSettings : IIndexerSettings
    {
        public NzbDroneValidationResult Validate()
        {
            throw new NotImplementedException();
        }

        public string BaseUrl { get; set; }

        public IEnumerable<int> MultiLanguages { get; set; }
        public IEnumerable<int> FailDownloads { get; set; }
    }
}
