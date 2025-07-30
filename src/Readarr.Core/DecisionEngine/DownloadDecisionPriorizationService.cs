using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Configuration;
using Readarr.Core.Profiles.Delay;

namespace Readarr.Core.DecisionEngine
{
    public interface IPrioritizeDownloadDecision
    {
        List<DownloadDecision> PrioritizeDecisions(List<DownloadDecision> decisions);
    }

    public class DownloadDecisionPriorizationService : IPrioritizeDownloadDecision
    {
        private readonly IConfigService _configService;
        private readonly IDelayProfileService _delayProfileService;

        public DownloadDecisionPriorizationService(IConfigService configService, IDelayProfileService delayProfileService)
        {
            _configService = configService;
            _delayProfileService = delayProfileService;
        }

        public List<DownloadDecision> PrioritizeDecisions(List<DownloadDecision> decisions)
        {
            return decisions.Where(c => c.RemoteBook.Author != null)
                            .GroupBy(c => c.RemoteBook.Author.Id, (authorId, downloadDecisions) =>
                                {
                                    return downloadDecisions.OrderByDescending(decision => decision, new DownloadDecisionComparer(_configService, _delayProfileService));
                                })
                            .SelectMany(c => c)
                            .Union(decisions.Where(c => c.RemoteBook.Author == null))
                            .ToList();
        }
    }
}
