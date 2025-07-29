using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine
{
    public class DownloadDecision
    {
        public RemoteBook RemoteBook { get; private set; }
        public IEnumerable<Rejection> Rejections { get; private set; }

        public bool Approved => !Rejections.Any();

        public bool TemporarilyRejected
        {
            get
            {
                return Rejections.Any() && Rejections.All(r => r.Type == RejectionType.Temporary);
            }
        }

        public bool Rejected
        {
            get
            {
                return Rejections.Any() && Rejections.Any(r => r.Type == RejectionType.Permanent);
            }
        }

        public DownloadDecision(RemoteBook remoteBook, params Rejection[] rejections)
        {
            RemoteBook = remoteBook;
            Rejections = rejections.ToList();
        }

        public override string ToString()
        {
            if (Approved)
            {
                return "[OK] " + RemoteBook;
            }

            return "[Rejected " + Rejections.Count() + "]" + RemoteBook;
        }
    }
}
