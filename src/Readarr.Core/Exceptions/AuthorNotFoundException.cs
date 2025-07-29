using System;

namespace Readarr.Core.Exceptions
{
    public class AuthorNotFoundException : Exception
    {
        public string ForeignAuthorId { get; set; }

        public AuthorNotFoundException(string foreignAuthorId)
            : base($"Author with ID {foreignAuthorId} was not found")
        {
            ForeignAuthorId = foreignAuthorId;
        }

        public AuthorNotFoundException(string foreignAuthorId, string message)
            : base(message)
        {
            ForeignAuthorId = foreignAuthorId;
        }
    }
}