using Readarr.Core.DecisionEngine;

namespace Readarr.Core.MediaFiles.BookImport
{
    public class ImportRejection : Rejection
    {
        public ImportRejection(string reason, RejectionType type = RejectionType.Permanent)
            : base(reason, type)
        {
        }
    }
}