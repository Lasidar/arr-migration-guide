using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using NLog;
using Readarr.Common.EnsureThat;
using Readarr.Core.Books.Events;
using Readarr.Core.Exceptions;
using Readarr.Core.MetadataSource;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public class AddBookService : IAddBookService
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IProvideBookInfo _bookInfo;
        private readonly IAddBookValidator _addBookValidator;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public AddBookService(IBookService bookService,
                             IAuthorService authorService,
                             IProvideBookInfo bookInfo,
                             IAddBookValidator addBookValidator,
                             IEventAggregator eventAggregator,
                             Logger logger)
        {
            _bookService = bookService;
            _authorService = authorService;
            _bookInfo = bookInfo;
            _addBookValidator = addBookValidator;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public Book AddBook(Book newBook)
        {
            Ensure.That(newBook, () => newBook).IsNotNull();

            newBook = AddSkyhookData(newBook);
            newBook = SetPropertiesAndValidate(newBook);

            _logger.Info("Adding Book {0}", newBook);

            _bookService.AddBook(newBook);

            return newBook;
        }

        private Book AddSkyhookData(Book newBook)
        {
            var book = newBook;

            try
            {
                var bookInfo = _bookInfo.GetBookInfo(newBook.Metadata.Value.ForeignBookId);
                
                book.Metadata = bookInfo.Metadata;
                book.Editions = bookInfo.Editions;
            }
            catch (BookNotFoundException)
            {
                _logger.Error("Foreign ID {0} was not found, it may have been removed from Goodreads.", newBook.Metadata.Value.ForeignBookId);

                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("ForeignBookId", $"A book with this ID was not found. It may have been removed from Goodreads.", newBook.Metadata.Value.ForeignBookId)
                });
            }

            return book;
        }

        private Book SetPropertiesAndValidate(Book newBook)
        {
            // Ensure we have an author
            if (newBook.AuthorId == 0)
            {
                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("AuthorId", "Author must be specified")
                });
            }

            var author = _authorService.GetAuthor(newBook.AuthorId);
            if (author == null)
            {
                throw new ValidationException(new List<ValidationFailure>
                {
                    new ValidationFailure("AuthorId", "Invalid Author")
                });
            }

            newBook.CleanTitle = newBook.Metadata.Value.Title.CleanBookTitle();
            newBook.SortTitle = newBook.Metadata.Value.SortTitle;
            newBook.Added = DateTime.UtcNow;

            if (newBook.AddOptions != null && newBook.AddOptions.Monitor == MonitorTypes.None)
            {
                newBook.Monitored = false;
            }

            var validationResult = _addBookValidator.Validate(newBook);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            return newBook;
        }
    }

    public interface IAddBookValidator
    {
        ValidationResult Validate(Book instance);
    }

    public class AddBookValidator : AbstractValidator<Book>, IAddBookValidator
    {
        public AddBookValidator()
        {
            RuleFor(b => b.Metadata).NotNull().SetValidator(new BookMetadataValidator());
            RuleFor(b => b.AuthorId).GreaterThan(0);
        }
    }

    public class BookMetadataValidator : AbstractValidator<BookMetadata>
    {
        public BookMetadataValidator()
        {
            RuleFor(b => b.ForeignBookId).NotEmpty();
            RuleFor(b => b.Title).NotEmpty();
        }
    }
}