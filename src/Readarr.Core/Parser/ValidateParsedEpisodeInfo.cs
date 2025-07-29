using NLog;
using Readarr.Common.Instrumentation;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.Parser
{
    public static class ValidateParsedEpisodeInfo
    {
        private static readonly Logger Logger = ReadarrLogger.GetLogger(typeof(ValidateParsedEpisodeInfo));

        public static bool ValidateForSeriesType(ParsedEpisodeInfo parsedEpisodeInfo, Series series, bool warnIfInvalid = true)
        {
            if (parsedEpisodeInfo.IsDaily && series.SeriesType == SeriesTypes.Standard)
            {
                var message = $"Found daily-style episode for non-daily series: {series}";

                if (warnIfInvalid)
                {
                    Logger.Warn(message);
                }
                else
                {
                    Logger.Debug(message);
                }

                return false;
            }

            return true;
        }
    }
}
