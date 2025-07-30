using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Core.Books.Events;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public interface IBookService
    {
        Book GetBook(int bookId);
        List<Book> GetBooks(IEnumerable<int> bookIds);
        List<Book> GetBooksByAuthor(int authorId);
        List<Book> GetBooksByAuthorMetadataId(int authorMetadataId);
        List<Book> GetBooksBySeries(int seriesId);
        Book AddBook(Book newBook);
        List<Book> AddBooks(List<Book> newBooks);
        Book FindByForeignBookId(string foreignBookId);
        Book FindByIsbn(string isbn);
        Book FindByTitle(int authorMetadataId, string title);
        List<Book> FindByTitle(string title);
        void DeleteBook(int bookId, bool deleteFiles, bool addImportListExclusion);
        void DeleteBooks(List<int> bookIds, bool deleteFiles, bool addImportListExclusion);
        List<Book> GetAllBooks();
        Book UpdateBook(Book book);
        List<Book> UpdateBooks(List<Book> books);
        void SetBookMonitored(int bookId, bool monitored);
        void SetBooksMonitored(List<int> bookIds, bool monitored);
        PagingSpec<Book> BooksWithoutFiles(PagingSpec<Book> pagingSpec);
        List<Book> BooksBetweenDates(DateTime start, DateTime end, bool includeUnmonitored);
        List<Book> AuthorBooksBetweenDates(Author author, DateTime start, DateTime end, bool includeUnmonitored);
        void InsertMany(List<Book> books);
        void UpdateMany(List<Book> books);
        void DeleteMany(List<Book> books);
        void RemoveAddOptions(Book book);
        bool ValidateIsbn13(string isbn);
        bool ValidateIsbn10(string isbn);
    }

    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public BookService(IBookRepository bookRepository,
                          IEventAggregator eventAggregator,
                          Logger logger)
        {
            _bookRepository = bookRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public Book GetBook(int bookId)
        {
            return _bookRepository.Get(bookId);
        }

        public List<Book> GetBooks(IEnumerable<int> bookIds)
        {
            return _bookRepository.Get(bookIds).ToList();
        }

        public List<Book> GetBooksByAuthor(int authorId)
        {
            return _bookRepository.GetBooks(authorId);
        }

        public List<Book> GetBooksByAuthorMetadataId(int authorMetadataId)
        {
            return _bookRepository.GetBooksByAuthorMetadataId(authorMetadataId);
        }

        public List<Book> GetBooksBySeries(int seriesId)
        {
            return _bookRepository.GetBooksBySeriesId(seriesId);
        }

        public Book AddBook(Book newBook)
        {
            _bookRepository.Insert(newBook);
            _eventAggregator.PublishEvent(new BookAddedEvent(GetBook(newBook.Id)));

            return newBook;
        }

        public List<Book> AddBooks(List<Book> newBooks)
        {
            _bookRepository.InsertMany(newBooks);
            _eventAggregator.PublishEvent(new BooksImportedEvent(newBooks.Select(b => b.Id).ToList()));

            return newBooks;
        }

        public Book FindByForeignBookId(string foreignBookId)
        {
            return _bookRepository.FindByForeignBookId(foreignBookId);
        }

        public Book FindByIsbn(string isbn)
        {
            return _bookRepository.FindByIsbn(isbn);
        }

        public Book FindByTitle(int authorMetadataId, string title)
        {
            return _bookRepository.FindByTitle(authorMetadataId, title);
        }

        public List<Book> FindByTitle(string title)
        {
            return _bookRepository.FindByTitle(title);
        }

        public void DeleteBook(int bookId, bool deleteFiles, bool addImportListExclusion)
        {
            var book = _bookRepository.Get(bookId);
            _bookRepository.Delete(bookId);
            _eventAggregator.PublishEvent(new BookDeletedEvent(book, deleteFiles, addImportListExclusion));
        }

        public void DeleteBooks(List<int> bookIds, bool deleteFiles, bool addImportListExclusion)
        {
            var books = _bookRepository.Get(bookIds).ToList();
            _bookRepository.DeleteMany(bookIds);

            foreach (var book in books)
            {
                _eventAggregator.PublishEvent(new BookDeletedEvent(book, deleteFiles, addImportListExclusion));
            }
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.All().ToList();
        }

        public Book UpdateBook(Book book)
        {
            var storedBook = GetBook(book.Id);
            var updatedBook = _bookRepository.Update(book);
            _eventAggregator.PublishEvent(new BookEditedEvent(updatedBook, storedBook));

            return updatedBook;
        }

        public List<Book> UpdateBooks(List<Book> books)
        {
            _logger.Debug("Updating {0} books", books.Count);
            _bookRepository.UpdateMany(books);
            _logger.Debug("{0} books updated", books.Count);

            return books;
        }

        public void SetBookMonitored(int bookId, bool monitored)
        {
            _bookRepository.SetMonitoredFlag(bookId, monitored);
        }

        public void SetBooksMonitored(List<int> bookIds, bool monitored)
        {
            _bookRepository.SetMonitoredFlagForBooks(bookIds, monitored);
        }

        public PagingSpec<Book> BooksWithoutFiles(PagingSpec<Book> pagingSpec)
        {
            var booksResult = _bookRepository.BooksWithoutFiles(pagingSpec);

            return booksResult;
        }

        public List<Book> BooksBetweenDates(DateTime start, DateTime end, bool includeUnmonitored)
        {
            return _bookRepository.BooksBetweenDates(start, end, includeUnmonitored);
        }

        public List<Book> AuthorBooksBetweenDates(Author author, DateTime start, DateTime end, bool includeUnmonitored)
        {
            return _bookRepository.AuthorBooksBetweenDates(author, start, end, includeUnmonitored);
        }

        public void InsertMany(List<Book> books)
        {
            _bookRepository.InsertMany(books);
        }

        public void UpdateMany(List<Book> books)
        {
            _bookRepository.UpdateMany(books);
        }

        public void DeleteMany(List<Book> books)
        {
            _bookRepository.DeleteMany(books.Select(b => b.Id).ToList());
        }

        public void RemoveAddOptions(Book book)
        {
            _bookRepository.SetFields(book, b => b.AddOptions);
        }

        public bool ValidateIsbn13(string isbn)
        {
            if (string.IsNullOrEmpty(isbn) || isbn.Length != 13)
                return false;
            
            // Check if all characters are digits
            if (!isbn.All(char.IsDigit))
                return false;
            
            // Calculate checksum
            var sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);
            }
            
            var checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == (isbn[12] - '0');
        }

        public bool ValidateIsbn10(string isbn)
        {
            if (string.IsNullOrEmpty(isbn) || isbn.Length != 10)
                return false;
            
            // First 9 characters must be digits
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(isbn[i]))
                    return false;
            }
            
            // Last character can be digit or X
            if (!char.IsDigit(isbn[9]) && isbn[9] != 'X' && isbn[9] != 'x')
                return false;
            
            // Calculate checksum
            var sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (isbn[i] - '0') * (10 - i);
            }
            
            var checkDigit = isbn[9] == 'X' || isbn[9] == 'x' ? 10 : (isbn[9] - '0');
            sum += checkDigit;
            
            return sum % 11 == 0;
        }
    }
}