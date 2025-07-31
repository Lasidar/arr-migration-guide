using System.Collections.Generic;
using Readarr.Core.Extras.Files;
using Readarr.Core.Books;

namespace Readarr.Core.Extras
{
    public interface IImportExistingExtraFiles
    {
        int Order { get; }
        IEnumerable<ExtraFile> ProcessFiles(Series series, List<string> filesOnDisk, List<string> importedFiles, string fileNameBeforeRename);
    }
}
