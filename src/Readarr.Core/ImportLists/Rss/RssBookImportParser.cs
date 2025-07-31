using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Indexers;
using Readarr.Core.Indexers.Exceptions;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists.Rss
{
    public class RssBookImportParser : IParseImportListResponse
    {
        private readonly Logger _logger;

        public RssBookImportParser(Logger logger)
        {
            _logger = logger;
        }

        public IList<ImportListItemInfo> ParseResponse(ImportListResponse importListResponse)
        {
            var items = new List<ImportListItemInfo>();

            if (!PreProcess(importListResponse))
            {
                return items;
            }

            var document = LoadXmlDocument(importListResponse);
            var root = document.Root;

            if (root == null || root.Name.LocalName != "rss")
            {
                throw new UnsupportedFeedException("Invalid RSS feed");
            }

            var channel = root.Element("channel");
            if (channel == null)
            {
                throw new UnsupportedFeedException("RSS feed does not contain a channel element");
            }

            var rssItems = channel.Elements("item");

            foreach (var item in rssItems)
            {
                try
                {
                    var bookInfo = ProcessItem(item);

                    items.AddIfNotNull(bookInfo);
                }
                catch (UnsupportedFeedException ex)
                {
                    _logger.Warn(ex, "Unable to parse book from RSS feed");
                }
            }

            return items.Cast<ImportListItemInfo>().ToList();
        }

        protected virtual bool PreProcess(ImportListResponse importListResponse)
        {
            if (importListResponse.HttpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ImportListException(importListResponse, "Request resulted in an unexpected StatusCode [{0}]", importListResponse.HttpResponse.StatusCode);
            }

            if (importListResponse.HttpResponse.Headers.ContentType != null && importListResponse.HttpResponse.Headers.ContentType.Contains("text/html"))
            {
                throw new UnsupportedFeedException("Feed content is HTML, not RSS");
            }

            return true;
        }

        protected virtual XDocument LoadXmlDocument(ImportListResponse importListResponse)
        {
            try
            {
                var content = XmlCleaner.ReplaceEntities(importListResponse.Content);
                content = XmlCleaner.ReplaceUnicode(content);

                using (var xmlTextReader = XmlReader.Create(new System.IO.StringReader(content), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, IgnoreComments = true }))
                {
                    return XDocument.Load(xmlTextReader);
                }
            }
            catch (XmlException ex)
            {
                var message = string.Format("Unable to parse XML: {0} [{1}]", ex.Message, ex.GetType());
                _logger.Debug(ex, message);

                throw new UnsupportedFeedException(message, ex);
            }
        }

        protected virtual ImportListBookInfo ProcessItem(XElement item)
        {
            var title = item.TryGetValue("title", "Unknown");
            var author = ParseAuthor(item);
            var isbn = ParseIsbn(item);
            var goodreadsId = ParseGoodreadsId(item);
            var releaseDate = ParseDate(item);

            var info = new ImportListBookInfo
            {
                Title = title,
                AuthorName = author,
                Isbn = isbn,
                GoodreadsId = goodreadsId,
                ReleaseDate = releaseDate
            };

            return info;
        }

        protected virtual string ParseAuthor(XElement item)
        {
            var author = item.TryGetValue("author");
            if (author.IsNullOrWhiteSpace())
            {
                author = item.TryGetValue("dc:creator", "http://purl.org/dc/elements/1.1/");
            }

            return author ?? "Unknown Author";
        }

        protected virtual string ParseIsbn(XElement item)
        {
            var isbn = item.TryGetValue("isbn");
            if (isbn.IsNullOrWhiteSpace())
            {
                // Try to extract from description or other fields
                var description = item.TryGetValue("description");
                if (!description.IsNullOrWhiteSpace())
                {
                    var match = System.Text.RegularExpressions.Regex.Match(description, @"ISBN[:\s]*(\d{10}|\d{13})");
                    if (match.Success)
                    {
                        isbn = match.Groups[1].Value;
                    }
                }
            }

            return isbn;
        }

        protected virtual string ParseGoodreadsId(XElement item)
        {
            var link = item.TryGetValue("link");
            if (!link.IsNullOrWhiteSpace() && link.Contains("goodreads.com"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(link, @"goodreads\.com/book/show/(\d+)");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        protected virtual DateTime? ParseDate(XElement item)
        {
            var dateString = item.TryGetValue("pubDate");
            if (!dateString.IsNullOrWhiteSpace())
            {
                if (DateTime.TryParse(dateString, out var date))
                {
                    return date;
                }
            }

            return null;
        }
    }
}