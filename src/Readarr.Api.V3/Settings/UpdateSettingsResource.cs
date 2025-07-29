using Readarr.Core.Update;
using Readarr.Http.REST;

namespace Readarr.Api.V3.Settings;

public class UpdateSettingsResource : RestResource
{
    public string? Branch { get; set; }
    public bool UpdateAutomatically { get; set; }
    public UpdateMechanism UpdateMechanism { get; set; }
    public string? UpdateScriptPath { get; set; }
}
