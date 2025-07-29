using System.Collections.Generic;
using System.Text.Json.Serialization;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.Books.Commands
{
    public class RefreshSeriesCommand : Command
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int AuthorId
        {
            get => 0;
            set
            {
                if (AuthorIds.Empty())
                {
                    AuthorIds.Add(value);
                }
            }
        }

        public List<int> AuthorIds { get; set; }
        public bool IsNewSeries { get; set; }

        public RefreshSeriesCommand()
        {
            AuthorIds = new List<int>();
        }

        public RefreshSeriesCommand(List<int> seriesIds, bool isNewSeries = false)
        {
            AuthorIds = seriesIds;
            IsNewSeries = isNewSeries;
        }

        public override bool SendUpdatesToClient => true;

        public override bool UpdateScheduledTask => AuthorIds.Empty();

        public override bool IsLongRunning => true;

        public override string CompletionMessage => "Completed";
    }
}
