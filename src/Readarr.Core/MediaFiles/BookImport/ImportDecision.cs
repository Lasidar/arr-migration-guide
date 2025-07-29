using System.Collections.Generic;
using System.Linq;
using Readarr.Core.DecisionEngine;

namespace Readarr.Core.MediaFiles.BookImport
{
    public class ImportDecision<T>
    {
        public T Item { get; private set; }
        public IEnumerable<Rejection> Rejections { get; private set; }

        public bool Approved => !Rejections.Any();

        public ImportDecision(T item, params Rejection[] rejections)
        {
            Item = item;
            Rejections = rejections.ToList();
        }

        public void Reject(Rejection rejection)
        {
            var rejections = Rejections.ToList();
            rejections.Add(rejection);
            Rejections = rejections;
        }
    }
}