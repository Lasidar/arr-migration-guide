using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.DecisionEngine;
using Readarr.Core.Download;
using Readarr.Core.MediaFiles.BookImport;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles
{
    public interface IDownloadedBooksImportService
    {
        List<ImportResult> ProcessRootFolder(DirectoryInfo directoryInfo);
        List<ImportResult> ProcessPath(string path, ImportMode importMode = ImportMode.Auto, Author author = null, DownloadClientItem downloadClientItem = null);
        bool ShouldDeleteFolder(DirectoryInfo directoryInfo, Author author);
    }

    public class DownloadedBooksImportService : IDownloadedBooksImportService
    {
        private readonly IDiskProvider _diskProvider;
        private readonly IDiskScanService _diskScanService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IParsingService _parsingService;
        private readonly IMakeImportDecision _importDecisionMaker;
        private readonly IImportApprovedBooks _importApprovedBooks;
        private readonly Logger _logger;

        public DownloadedBooksImportService(IDiskProvider diskProvider,
            IDiskScanService diskScanService,
            IBookService bookService,
            IAuthorService authorService,
            IParsingService parsingService,
            IMakeImportDecision importDecisionMaker,
            IImportApprovedBooks importApprovedBooks,
            Logger logger)
        {
            _diskProvider = diskProvider;
            _diskScanService = diskScanService;
            _bookService = bookService;
            _authorService = authorService;
            _parsingService = parsingService;
            _importDecisionMaker = importDecisionMaker;
            _importApprovedBooks = importApprovedBooks;
            _logger = logger;
        }

        public List<ImportResult> ProcessRootFolder(DirectoryInfo directoryInfo)
        {
            var results = new List<ImportResult>();

            foreach (var subFolder in _diskProvider.GetDirectories(directoryInfo.FullName))
            {
                var folderResults = ProcessFolder(new DirectoryInfo(subFolder), ImportMode.Auto, null);
                results.AddRange(folderResults);
            }

            foreach (var file in _diskProvider.GetFiles(directoryInfo.FullName, false))
            {
                var fileResults = ProcessFile(new FileInfo(file), ImportMode.Auto, null);
                results.AddRange(fileResults);
            }

            return results;
        }

        public List<ImportResult> ProcessPath(string path, ImportMode importMode = ImportMode.Auto, Author author = null, DownloadClientItem downloadClientItem = null)
        {
            if (_diskProvider.FolderExists(path))
            {
                return ProcessFolder(new DirectoryInfo(path), importMode, author, downloadClientItem);
            }

            if (_diskProvider.FileExists(path))
            {
                return ProcessFile(new FileInfo(path), importMode, author, downloadClientItem);
            }

            _logger.Error("Import failed, path does not exist or is not accessible by Readarr: {0}", path);
            return new List<ImportResult>();
        }

        public bool ShouldDeleteFolder(DirectoryInfo directoryInfo, Author author)
        {
            var bookFiles = _diskScanService.GetBookFiles(directoryInfo.FullName);
            var rarFiles = _diskProvider.GetFiles(directoryInfo.FullName, true).Where(f => Path.GetExtension(f).Equals(".rar", StringComparison.OrdinalIgnoreCase));

            foreach (var bookFile in bookFiles)
            {
                var bookParseResult = _parsingService.GetBook(bookFile);

                if (bookParseResult == null)
                {
                    _logger.Warn("Unable to parse book file on import: [{0}]", bookFile);
                    return false;
                }

                var book = _bookService.GetBook(bookParseResult.Id);

                if (book == null)
                {
                    _logger.Warn("Couldn't find book {0} in database", bookParseResult);
                    return false;
                }

                if (author != null && author.Id != book.AuthorMetadataId)
                {
                    _logger.Warn("Book {0} doesn't belong to author {1}", book, author);
                    return false;
                }
            }

            if (rarFiles.Any(f => _diskProvider.GetFileSize(f) > 10.Megabytes()))
            {
                _logger.Warn("RAR file detected, will not delete folder: {0}", directoryInfo.FullName);
                return false;
            }

            return true;
        }

        private List<ImportResult> ProcessFolder(DirectoryInfo directoryInfo, ImportMode importMode, Author author, DownloadClientItem downloadClientItem = null)
        {
            var cleanedUpName = GetCleanedUpFolderName(directoryInfo.Name);
            var bookFiles = _diskScanService.GetBookFiles(directoryInfo.FullName);
            var decisions = _importDecisionMaker.GetImportDecisions(bookFiles, author, null, directoryInfo, SceneSource(author, directoryInfo.Name), downloadClientItem != null);
            var importResults = _importApprovedBooks.Import(decisions, true, downloadClientItem, importMode);

            if (importMode == ImportMode.Auto)
            {
                importMode = (downloadClientItem == null || downloadClientItem.CanMoveFiles) ? ImportMode.Move : ImportMode.Copy;
            }

            if (importMode == ImportMode.Move && importResults.Any(i => i.Result == ImportResultType.Imported) && ShouldDeleteFolder(directoryInfo, author))
            {
                _logger.Debug("Deleting folder after importing valid files");
                _diskProvider.DeleteFolder(directoryInfo.FullName, true);
            }

            return importResults;
        }

        private List<ImportResult> ProcessFile(FileInfo fileInfo, ImportMode importMode, Author author, DownloadClientItem downloadClientItem = null)
        {
            var decisions = _importDecisionMaker.GetImportDecisions(new List<string> { fileInfo.FullName }, author, null, null, SceneSource(author, fileInfo.Name), downloadClientItem != null);
            return _importApprovedBooks.Import(decisions, true, downloadClientItem, importMode);
        }

        private string GetCleanedUpFolderName(string folder)
        {
            folder = folder.Replace("_UNPACK_", "")
                           .Replace("_FAILED_", "");

            return folder;
        }

        private bool SceneSource(Author author, string folder)
        {
            return !(author != null && author.Path.IsParentPath(folder));
        }
    }
}