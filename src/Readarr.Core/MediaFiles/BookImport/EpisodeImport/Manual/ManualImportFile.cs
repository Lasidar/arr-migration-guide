using System;
using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Languages;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;

namespace Readarr.Core.MediaFiles.BookImport.Manual
{
    public class ManualImportFile : IEquatable<ManualImportFile>
    {
        public string Path { get; set; }
        public string FolderName { get; set; }
        public int SeriesId { get; set; }
        public List<int> EpisodeIds { get; set; }
        public int? EpisodeFileId { get; set; }
        public QualityModel Quality { get; set; }
        public List<Language> Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public int IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public string DownloadId { get; set; }

        public bool Equals(ManualImportFile other)
        {
            if (other == null)
            {
                return false;
            }

            return Path.PathEquals(other.Path);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Path.PathEquals(((ManualImportFile)obj).Path);
        }

        public override int GetHashCode()
        {
            return Path != null ? Path.GetHashCode() : 0;
        }
    }
}
