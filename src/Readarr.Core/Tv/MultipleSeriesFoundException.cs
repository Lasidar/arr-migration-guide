using System.Collections.Generic;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Tv
{
    public class MultipleSeriesFoundException : ReadarrException
    {
        public List<Series> Series { get; set; }

        public MultipleSeriesFoundException(List<Series> series, string message, params object[] args)
            : base(message, args)
        {
            Series = series;
        }
    }
}
