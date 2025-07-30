using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using NLog;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles
{
    public interface IBookMetadataExtractor
    {
        BookMetadata ExtractMetadata(string filePath);
    }

    public class BookMetadataExtractor : IBookMetadataExtractor
    {
        private readonly Logger _logger;

        public BookMetadataExtractor(Logger logger)
        {
            _logger = logger;
        }

        public BookMetadata ExtractMetadata(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.Warn("File does not exist: {0}", filePath);
                return null;
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            try
            {
                return extension switch
                {
                    ".epub" => ExtractEpubMetadata(filePath),
                    ".pdf" => ExtractPdfMetadata(filePath),
                    ".mobi" or ".azw" or ".azw3" => ExtractMobiMetadata(filePath),
                    _ => ExtractBasicMetadata(filePath)
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to extract metadata from {0}", filePath);
                return ExtractBasicMetadata(filePath);
            }
        }

        private BookMetadata ExtractEpubMetadata(string filePath)
        {
            var metadata = new BookMetadata();

            try
            {
                // EPUB files are ZIP archives containing metadata in OPF files
                using (var archive = System.IO.Compression.ZipFile.OpenRead(filePath))
                {
                    var opfEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".opf", StringComparison.OrdinalIgnoreCase));
                    
                    if (opfEntry != null)
                    {
                        using (var stream = opfEntry.Open())
                        {
                            var doc = XDocument.Load(stream);
                            var ns = doc.Root.GetDefaultNamespace();
                            var metadata = doc.Root.Element(ns + "metadata");

                            if (metadata != null)
                            {
                                var result = new BookMetadata
                                {
                                    Title = metadata.Elements().FirstOrDefault(e => e.Name.LocalName == "title")?.Value,
                                    Publisher = metadata.Elements().FirstOrDefault(e => e.Name.LocalName == "publisher")?.Value,
                                    Language = metadata.Elements().FirstOrDefault(e => e.Name.LocalName == "language")?.Value
                                };

                                // Extract ISBN
                                var identifiers = metadata.Elements().Where(e => e.Name.LocalName == "identifier");
                                foreach (var id in identifiers)
                                {
                                    var scheme = id.Attribute("scheme")?.Value ?? id.Attribute(ns + "scheme")?.Value;
                                    if (scheme?.ToLower() == "isbn")
                                    {
                                        result.Isbn13 = id.Value;
                                        break;
                                    }
                                }

                                // Extract release date
                                var dateStr = metadata.Elements().FirstOrDefault(e => e.Name.LocalName == "date")?.Value;
                                if (DateTime.TryParse(dateStr, out var date))
                                {
                                    result.ReleaseDate = date;
                                }

                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to extract EPUB metadata from {0}", filePath);
            }

            return ExtractBasicMetadata(filePath);
        }

        private BookMetadata ExtractPdfMetadata(string filePath)
        {
            // PDF metadata extraction would require a PDF library
            // For now, return basic metadata
            _logger.Debug("PDF metadata extraction not implemented for {0}", filePath);
            return ExtractBasicMetadata(filePath);
        }

        private BookMetadata ExtractMobiMetadata(string filePath)
        {
            // MOBI/AZW metadata extraction would require specific parsing
            // For now, return basic metadata
            _logger.Debug("MOBI/AZW metadata extraction not implemented for {0}", filePath);
            return ExtractBasicMetadata(filePath);
        }

        private BookMetadata ExtractBasicMetadata(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            
            // Try to parse filename for basic info
            // Common patterns: "Author - Title", "Title by Author", etc.
            var metadata = new BookMetadata();

            if (fileName.Contains(" - "))
            {
                var parts = fileName.Split(new[] { " - " }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    // Could be either "Author - Title" or "Title - Author"
                    // For now, assume "Author - Title"
                    metadata.Title = parts[1].Trim();
                }
            }
            else if (fileName.Contains(" by ", StringComparison.OrdinalIgnoreCase))
            {
                var parts = fileName.Split(new[] { " by " }, 2, StringSplitOptions.IgnoreCase);
                if (parts.Length == 2)
                {
                    metadata.Title = parts[0].Trim();
                }
            }
            else
            {
                metadata.Title = fileName;
            }

            return metadata;
        }
    }
}