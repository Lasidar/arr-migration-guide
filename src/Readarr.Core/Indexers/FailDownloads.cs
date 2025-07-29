using Readarr.Core.Annotations;

namespace Readarr.Core.Indexers;

public enum FailDownloads
{
    [FieldOption(Label = "Executables")]
    Executables = 0,

    [FieldOption(Label = "Potentially Dangerous")]
    PotentiallyDangerous = 1,

    [FieldOption(Label = "User Defined Extensions")]
    UserDefinedExtensions = 2
}
