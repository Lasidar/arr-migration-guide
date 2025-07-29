using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.DecisionEngine;
using Readarr.Core.MediaFiles.BookImport.Aggregation;
using Readarr.Core.MediaFiles.BookImport.Specifications;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;

namespace Readarr.Core.MediaFiles.BookImport
{
    public interface IImportDecisionMaker
    {
        List<ImportDecision<LocalBook>> GetImportDecisions(List<IFileInfo> bookFiles, Author author);
        List<ImportDecision<LocalBook>> GetImportDecisions(List<IFileInfo> bookFiles, Author author, FilterFilesType filter);
        List<ImportDecision<LocalBook>> GetImportDecisions(List<IFileInfo> bookFiles, Author author, FilterFilesType filter, bool includeExisting);
    }

    public class ImportDecisionMaker : IImportDecisionMaker
    {
        private readonly IEnumerable<IImportDecisionEngineSpecification<LocalBook>> _specifications;
        private readonly IParsingService _parsingService;
        private readonly IAugmentingService _augmentingService;
        private readonly IIdentificationService _identificationService;
        private readonly IDiskProvider _diskProvider;
        private readonly IDetectSample _detectSample;
        private readonly Logger _logger;

        public ImportDecisionMaker(IEnumerable<IImportDecisionEngineSpecification<LocalBook>> specifications,
                                   IParsingService parsingService,
                                   IAugmentingService augmentingService,
                                   IIdentificationService identificationService,
                                   IDiskProvider diskProvider,
                                   IDetectSample detectSample,
                                   Logger logger)
        {
            _specifications = specifications;
            _parsingService = parsingService;
            _augmentingService = augmentingService;
            _identificationService = identificationService;
            _diskProvider = diskProvider;
            _detectSample = detectSample;
            _logger = logger;
        }

        public List<ImportDecision<LocalBook>> GetImportDecisions(List<IFileInfo> bookFiles, Author author)
        {
            return GetImportDecisions(bookFiles, author, FilterFilesType.None);
        }

        public List<ImportDecision<LocalBook>> GetImportDecisions(List<IFileInfo> bookFiles, Author author, FilterFilesType filter)
        {
            return GetImportDecisions(bookFiles, author, filter, false);
        }

        public List<ImportDecision<LocalBook>> GetImportDecisions(List<IFileInfo> bookFiles, Author author, FilterFilesType filter, bool includeExisting)
        {
            var decisions = new List<ImportDecision<LocalBook>>();

            foreach (var file in bookFiles)
            {
                var localBook = new LocalBook
                {
                    Author = author,
                    FileInfo = file,
                    Path = file.FullName,
                    Size = file.Length,
                    Modified = file.LastWriteTimeUtc,
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Quality = new QualityModel(Quality.Unknown)
                };

                try
                {
                    // Parse book info from filename
                    var bookInfo = Parser.Parser.ParseBookTitle(file.Name);
                    
                    if (bookInfo != null)
                    {
                        localBook.BookInfo = bookInfo;
                        
                        // Try to find the book in the author's collection
                        if (bookInfo.BookTitle.IsNotNullOrWhiteSpace())
                        {
                            var book = _parsingService.GetBook(author, bookInfo.BookTitle);
                            if (book != null)
                            {
                                localBook.Book = book;
                            }
                        }
                    }

                    // Augment with additional information
                    _augmentingService.Augment(localBook, true);

                    // Identify the book if not already identified
                    if (localBook.Book == null)
                    {
                        _identificationService.Identify(localBook);
                    }

                    // Get decision
                    var decision = GetDecision(localBook, filter, includeExisting);
                    decisions.Add(decision);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Couldn't process book file {0}", file.FullName);

                    var decision = new ImportDecision<LocalBook>(localBook, new Rejection("Unexpected error processing file"));
                    decisions.Add(decision);
                }
            }

            return decisions;
        }

        private ImportDecision<LocalBook> GetDecision(LocalBook localBook, FilterFilesType filter, bool includeExisting)
        {
            var rejections = new List<Rejection>();

            // Check if it's a sample file
            if (_detectSample.IsSample(localBook))
            {
                rejections.Add(new Rejection("Sample file"));
            }

            // Apply specifications
            foreach (var specification in _specifications)
            {
                try
                {
                    var result = specification.IsSatisfiedBy(localBook);
                    if (!result.Accepted)
                    {
                        rejections.AddRange(result.Rejections);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Couldn't evaluate decision for {0}", localBook.Path);
                    rejections.Add(new Rejection($"{specification.GetType().Name}: {ex.Message}"));
                }
            }

            // Apply filter
            if (filter != FilterFilesType.None)
            {
                if (filter == FilterFilesType.Known && localBook.Book == null)
                {
                    rejections.Add(new Rejection("Book not found"));
                }
                else if (filter == FilterFilesType.Unknown && localBook.Book != null)
                {
                    rejections.Add(new Rejection("Book found"));
                }
            }

            // Check if already exists
            if (!includeExisting && localBook.ExistingFile)
            {
                rejections.Add(new Rejection("Book file already exists"));
            }

            return new ImportDecision<LocalBook>(localBook, rejections.ToArray());
        }
    }

    public enum FilterFilesType
    {
        None,
        Known,
        Unknown
    }
}