using System;

namespace Readarr.Core.Exceptions
{
    public class BookNotFoundException : Exception
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }

        public BookNotFoundException(int bookId) 
            : base($"Book with ID {bookId} was not found")
        {
            BookId = bookId;
        }

        public BookNotFoundException(string bookTitle) 
            : base($"Book '{bookTitle}' was not found")
        {
            BookTitle = bookTitle;
        }

        public BookNotFoundException(int bookId, string bookTitle) 
            : base($"Book '{bookTitle}' (ID: {bookId}) was not found")
        {
            BookId = bookId;
            BookTitle = bookTitle;
        }
    }

    public class AuthorNotFoundException : Exception
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }

        public AuthorNotFoundException(int authorId) 
            : base($"Author with ID {authorId} was not found")
        {
            AuthorId = authorId;
        }

        public AuthorNotFoundException(string authorName) 
            : base($"Author '{authorName}' was not found")
        {
            AuthorName = authorName;
        }

        public AuthorNotFoundException(int authorId, string authorName) 
            : base($"Author '{authorName}' (ID: {authorId}) was not found")
        {
            AuthorId = authorId;
            AuthorName = authorName;
        }
    }

    public class DuplicateAuthorException : Exception
    {
        public string AuthorName { get; set; }
        public int ExistingAuthorId { get; set; }

        public DuplicateAuthorException(string authorName, int existingAuthorId) 
            : base($"Author '{authorName}' already exists with ID {existingAuthorId}")
        {
            AuthorName = authorName;
            ExistingAuthorId = existingAuthorId;
        }
    }

    public class InvalidIsbnException : Exception
    {
        public string Isbn { get; set; }

        public InvalidIsbnException(string isbn) 
            : base($"Invalid ISBN: {isbn}")
        {
            Isbn = isbn;
        }

        public InvalidIsbnException(string isbn, string message) 
            : base($"Invalid ISBN '{isbn}': {message}")
        {
            Isbn = isbn;
        }
    }

    public class MetadataProviderException : Exception
    {
        public string ProviderName { get; set; }

        public MetadataProviderException(string providerName, string message) 
            : base($"Metadata provider '{providerName}' error: {message}")
        {
            ProviderName = providerName;
        }

        public MetadataProviderException(string providerName, string message, Exception innerException) 
            : base($"Metadata provider '{providerName}' error: {message}", innerException)
        {
            ProviderName = providerName;
        }
    }
}