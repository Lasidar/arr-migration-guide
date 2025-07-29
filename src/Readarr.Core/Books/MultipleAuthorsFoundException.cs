using System.Collections.Generic;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Books
{
    public class MultipleAuthorsFoundException : ReadarrException
    {
        public List<Author> Authors { get; set; }

        public MultipleAuthorsFoundException(List<Author> authors, string message, params object[] args)
            : base(message, args)
        {
            Authors = authors;
        }
    }
}