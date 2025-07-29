using System;

namespace Readarr.Common.Expansive
{
    public class CircularReferenceException : Exception
    {
        public CircularReferenceException(string message)
            : base(message)
        {
        }
    }
}
