using Readarr.Core.Messaging.Commands;

namespace Readarr.Core.Qualities.Commands
{
    public class ResetQualityDefinitionsCommand : Command
    {
        public bool ResetTitles { get; set; }

        public ResetQualityDefinitionsCommand(bool resetTitles = false)
        {
            ResetTitles = resetTitles;
        }
    }
}
