using System;
using System.Collections.Generic;
using System.Linq;

namespace Readarr.Core.MediaFiles
{
    public static class BookFileExtensions
    {
        private static readonly HashSet<string> EbookExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".epub",
            ".mobi", 
            ".azw",
            ".azw3",
            ".pdf",
            ".txt",
            ".rtf",
            ".doc",
            ".docx",
            ".html",
            ".htm",
            ".lit",
            ".fb2",    // FictionBook 2
            ".djvu",   // DjVu
            ".pdb",    // Palm Database
            ".prc",    // Palm Resource
            ".odt",    // OpenDocument Text
            ".cbr",    // Comic Book RAR
            ".cbz",    // Comic Book ZIP
            ".cb7",    // Comic Book 7z
            ".cbt",    // Comic Book TAR
            ".cba",    // Comic Book ACE
            ".chm",    // Compiled HTML Help
            ".oxps",   // OpenXPS
            ".xps"     // XML Paper Specification
        };

        private static readonly HashSet<string> AudiobookExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3",
            ".m4a",
            ".m4b",
            ".aac",
            ".flac",
            ".ogg",
            ".wma",
            ".wav",
            ".opus",
            ".ape",
            ".mka",
            ".aa",     // Audible Audio
            ".aax"     // Audible Enhanced Audio
        };

        private static readonly HashSet<string> AllBookExtensions = 
            new HashSet<string>(EbookExtensions.Union(AudiobookExtensions), StringComparer.OrdinalIgnoreCase);

        public static bool IsBookFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var extension = System.IO.Path.GetExtension(path);
            return AllBookExtensions.Contains(extension);
        }

        public static bool IsEbookFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var extension = System.IO.Path.GetExtension(path);
            return EbookExtensions.Contains(extension);
        }

        public static bool IsAudiobookFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var extension = System.IO.Path.GetExtension(path);
            return AudiobookExtensions.Contains(extension);
        }

        public static bool IsComicBookFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
            return extension == ".cbr" || extension == ".cbz" || extension == ".cb7" || 
                   extension == ".cbt" || extension == ".cba";
        }

        public static string GetBookFormat(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "Unknown";

            var extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
            
            return extension switch
            {
                ".epub" => "EPUB",
                ".mobi" => "MOBI",
                ".azw" or ".azw3" => "AZW",
                ".pdf" => "PDF",
                ".txt" => "TXT",
                ".rtf" => "RTF",
                ".doc" or ".docx" => "DOC",
                ".html" or ".htm" => "HTML",
                ".fb2" => "FB2",
                ".djvu" => "DJVU",
                ".cbr" or ".cbz" or ".cb7" or ".cbt" or ".cba" => "Comic",
                ".mp3" or ".m4a" or ".m4b" or ".aac" or ".flac" or ".ogg" => "Audiobook",
                ".aa" or ".aax" => "Audible",
                _ => "Unknown"
            };
        }

        public static List<string> GetAllSupportedExtensions()
        {
            return AllBookExtensions.ToList();
        }
    }
}