using System.Collections.Generic;
using System.Linq;

namespace Readarr.Core.MediaFiles.BookImport
{
    public class ImportResult
    {
        public ImportDecision<LocalBook> ImportDecision { get; private set; }
        public List<string> Errors { get; private set; }

        public ImportResult(ImportDecision<LocalBook> importDecision, params string[] errors)
        {
            ImportDecision = importDecision;
            Errors = errors.ToList();
        }
    }
}