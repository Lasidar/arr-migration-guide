using System.Collections.Generic;
using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.MediaFiles.Commands
{
    public class RescanFoldersCommand : Command
    {
        public List<string> Folders { get; set; }
        public FilterFilesType Filter { get; set; }
        public bool AddNewAuthors { get; set; }

        public RescanFoldersCommand()
        {
            Folders = new List<string>();
        }

        public RescanFoldersCommand(List<string> folders, FilterFilesType filter, bool addNewAuthors)
        {
            Folders = folders;
            Filter = filter;
            AddNewAuthors = addNewAuthors;
        }
    }

    public enum FilterFilesType
    {
        Known,
        Unknown
    }
}