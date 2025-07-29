using System.Collections.Generic;
using NzbDrone.Core.CustomFormats;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.MediaInfo;
using NzbDrone.Core.Qualities;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Organizer
{
    public interface IFilenameSampleService
    {
        SampleResult GetStandardSample(NamingConfig nameSpec);
        SampleResult GetMultiEpisodeSample(NamingConfig nameSpec);
        SampleResult GetDailySample(NamingConfig nameSpec);
        SampleResult GetAnimeSample(NamingConfig nameSpec);
        SampleResult GetAnimeMultiEpisodeSample(NamingConfig nameSpec);
        string GetSeriesFolderSample(NamingConfig nameSpec);
        string GetSeasonFolderSample(NamingConfig nameSpec);
        string GetSpecialsFolderSample(NamingConfig nameSpec);
    }

    public class FileNameSampleService : IFilenameSampleService
    {
        private readonly IBuildFileNames _buildFileNames;
        private static Series _standardSeries;
        private static Series _dailySeries;
        private static Series _animeSeries;
        private static Episode _episode1;
        private static Episode _episode2;
        private static Episode _episode3;
        private static List<Episode> _singleEpisode;
        private static List<Episode> _multiEpisodes;
        private static EditionFile _singleEditionFile;
        private static EditionFile _multiEditionFile;
        private static EditionFile _dailyEditionFile;
        private static EditionFile _animeEditionFile;
        private static EditionFile _animeMultiEditionFile;
        private static List<CustomFormat> _customFormats;

        public FileNameSampleService(IBuildFileNames buildFileNames)
        {
            _buildFileNames = buildFileNames;

            _standardSeries = new Series
            {
                SeriesType = SeriesTypes.Standard,
                Title = "The Series Title's!",
                Year = 2010,
                ImdbId = "tt12345",
                TvdbId = 12345,
                TvMazeId = 54321,
                TmdbId = 11223
            };

            _dailySeries = new Series
            {
                SeriesType = SeriesTypes.Daily,
                Title = "The Series Title's!",
                Year = 2010,
                ImdbId = "tt12345",
                TvdbId = 12345,
                TvMazeId = 54321,
                TmdbId = 11223
            };

            _animeSeries = new Series
            {
                SeriesType = SeriesTypes.Anime,
                Title = "The Series Title's!",
                Year = 2010,
                ImdbId = "tt12345",
                TvdbId = 12345,
                TvMazeId = 54321,
                TmdbId = 11223
            };

            _episode1 = new Episode
            {
                BookNumber = 1,
                EditionNumber = 1,
                Title = "Episode Title (1)",
                AirDate = "2013-10-30",
                AbsoluteEditionNumber = 1,
            };

            _episode2 = new Episode
            {
                BookNumber = 1,
                EditionNumber = 2,
                Title = "Episode Title (2)",
                AbsoluteEditionNumber = 2
            };

            _episode3 = new Episode
            {
                BookNumber = 1,
                EditionNumber = 3,
                Title = "Episode Title (3)",
                AbsoluteEditionNumber = 3
            };

            _singleEpisode = new List<Episode> { _episode1 };
            _multiEpisodes = new List<Episode> { _episode1, _episode2, _episode3 };

            var mediaInfo = new MediaInfoModel()
            {
                VideoFormat = "AVC",
                VideoBitDepth = 10,
                VideoColourPrimaries = "bt2020",
                VideoTransferCharacteristics = "HLG",
                AudioFormat = "DTS",
                AudioChannels = 6,
                AudioChannelPositions = "5.1",
                AudioLanguages = new List<string> { "ger" },
                Subtitles = new List<string> { "eng", "ger" }
            };

            var mediaInfoAnime = new MediaInfoModel()
            {
                VideoFormat = "AVC",
                VideoBitDepth = 10,
                VideoColourPrimaries = "BT.2020",
                VideoTransferCharacteristics = "HLG",
                AudioFormat = "DTS",
                AudioChannels = 6,
                AudioChannelPositions = "5.1",
                AudioLanguages = new List<string> { "jpn" },
                Subtitles = new List<string> { "jpn", "eng" }
            };

            _customFormats = new List<CustomFormat>
            {
                new CustomFormat
                {
                    Name = "Surround Sound",
                    IncludeCustomFormatWhenRenaming = true
                },
                new CustomFormat
                {
                    Name = "x264",
                    IncludeCustomFormatWhenRenaming = true
                }
            };

            _singleEditionFile = new EditionFile
            {
                Quality = new QualityModel(Quality.WEBDL1080p, new Revision(2)),
                RelativePath = "The.Series.Title's!.S01E01.1080p.WEBDL.x264-EVOLVE.mkv",
                SceneName = "The.Series.Title's!.S01E01.1080p.WEBDL.x264-EVOLVE",
                ReleaseGroup = "RlsGrp",
                MediaInfo = mediaInfo
            };

            _multiEditionFile = new EditionFile
            {
                Quality = new QualityModel(Quality.WEBDL1080p, new Revision(2)),
                RelativePath = "The.Series.Title's!.S01E01-E03.1080p.WEBDL.x264-EVOLVE.mkv",
                SceneName = "The.Series.Title's!.S01E01-E03.1080p.WEBDL.x264-EVOLVE",
                ReleaseGroup = "RlsGrp",
                MediaInfo = mediaInfo,
            };

            _dailyEditionFile = new EditionFile
            {
                Quality = new QualityModel(Quality.WEBDL1080p, new Revision(2)),
                RelativePath = "The.Series.Title's!.2013.10.30.1080p.WEBDL.x264-EVOLVE.mkv",
                SceneName = "The.Series.Title's!.2013.10.30.1080p.WEBDL.x264-EVOLVE",
                ReleaseGroup = "RlsGrp",
                MediaInfo = mediaInfo
            };

            _animeEditionFile = new EditionFile
            {
                Quality = new QualityModel(Quality.WEBDL1080p, new Revision(2)),
                RelativePath = "[RlsGroup] The Series Title's! - 001 [1080P].mkv",
                SceneName = "[RlsGroup] The Series Title's! - 001 [1080P]",
                ReleaseGroup = "RlsGrp",
                MediaInfo = mediaInfoAnime
            };

            _animeMultiEditionFile = new EditionFile
            {
                Quality = new QualityModel(Quality.WEBDL1080p, new Revision(2)),
                RelativePath = "[RlsGroup] The Series Title's! - 001 - 103 [1080p].mkv",
                SceneName = "[RlsGroup] The Series Title's! - 001 - 103 [1080p]",
                ReleaseGroup = "RlsGrp",
                MediaInfo = mediaInfoAnime
            };
        }

        public SampleResult GetStandardSample(NamingConfig nameSpec)
        {
            var result = new SampleResult
            {
                FileName = BuildSample(_singleEpisode, _standardSeries, _singleEditionFile, nameSpec, _customFormats),
                Series = _standardSeries,
                Episodes = _singleEpisode,
                EditionFile = _singleEditionFile
            };

            return result;
        }

        public SampleResult GetMultiEpisodeSample(NamingConfig nameSpec)
        {
            var result = new SampleResult
            {
                FileName = BuildSample(_multiEpisodes, _standardSeries, _multiEditionFile, nameSpec, _customFormats),
                Series = _standardSeries,
                Episodes = _multiEpisodes,
                EditionFile = _multiEditionFile
            };

            return result;
        }

        public SampleResult GetDailySample(NamingConfig nameSpec)
        {
            var result = new SampleResult
            {
                FileName = BuildSample(_singleEpisode, _dailySeries, _dailyEditionFile, nameSpec, _customFormats),
                Series = _dailySeries,
                Episodes = _singleEpisode,
                EditionFile = _dailyEditionFile
            };

            return result;
        }

        public SampleResult GetAnimeSample(NamingConfig nameSpec)
        {
            var result = new SampleResult
            {
                FileName = BuildSample(_singleEpisode, _animeSeries, _animeEditionFile, nameSpec, _customFormats),
                Series = _animeSeries,
                Episodes = _singleEpisode,
                EditionFile = _animeEditionFile
            };

            return result;
        }

        public SampleResult GetAnimeMultiEpisodeSample(NamingConfig nameSpec)
        {
            var result = new SampleResult
            {
                FileName = BuildSample(_multiEpisodes, _animeSeries, _animeMultiEditionFile, nameSpec, _customFormats),
                Series = _animeSeries,
                Episodes = _multiEpisodes,
                EditionFile = _animeMultiEditionFile
            };

            return result;
        }

        public string GetSeriesFolderSample(NamingConfig nameSpec)
        {
            return _buildFileNames.GetSeriesFolder(_standardSeries, nameSpec);
        }

        public string GetSeasonFolderSample(NamingConfig nameSpec)
        {
            return _buildFileNames.GetSeasonFolder(_standardSeries, _episode1.BookNumber, nameSpec);
        }

        public string GetSpecialsFolderSample(NamingConfig nameSpec)
        {
            return _buildFileNames.GetSeasonFolder(_standardSeries, 0, nameSpec);
        }

        private string BuildSample(List<Episode> episodes, Series series, EditionFile episodeFile, NamingConfig nameSpec, List<CustomFormat> customFormats)
        {
            try
            {
                return _buildFileNames.BuildFileName(episodes, series, episodeFile, "", nameSpec, customFormats);
            }
            catch (NamingFormatException)
            {
                return string.Empty;
            }
        }
    }
}
