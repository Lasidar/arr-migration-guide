using System;

namespace Readarr.Core.Exceptions
{
    public class BookNotFoundException : Exception
    {
        public string ForeignBookId { get; set; }

        public BookNotFoundException(string foreignBookId)
            : base($"Book with ID {foreignBookId} was not found")
        {
            ForeignBookId = foreignBookId;
        }

        public BookNotFoundException(string foreignBookId, string message)
            : base(message)
        {
            ForeignBookId = foreignBookId;
        }
    }
}