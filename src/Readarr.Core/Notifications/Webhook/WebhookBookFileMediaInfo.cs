using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.MediaInfo;

namespace Readarr.Core.Notifications.Webhook
{
    public class WebhookBookFileMediaInfo
    {
        public WebhookBookFileMediaInfo()
        {
        }

        public WebhookBookFileMediaInfo(BookFile bookFile)
        {
            // For audiobooks
            if (bookFile.MediaInfo != null)
            {
                AudioChannels = MediaInfoFormatter.FormatAudioChannels(bookFile.MediaInfo);
                AudioCodec = MediaInfoFormatter.FormatAudioCodec(bookFile.MediaInfo, bookFile.SceneName);
                AudioLanguages = bookFile.MediaInfo.AudioLanguages?.Distinct().ToList() ?? new List<string>();
                AudioBitrate = bookFile.MediaInfo.AudioBitrate;
                AudioSampleRate = bookFile.MediaInfo.AudioSampleRate;
                Duration = bookFile.MediaInfo.RunTime;
            }

            // Book-specific metadata
            Format = System.IO.Path.GetExtension(bookFile.Path)?.TrimStart('.').ToUpperInvariant();
        }

        // Audio properties (for audiobooks)
        public decimal AudioChannels { get; set; }
        public string AudioCodec { get; set; }
        public List<string> AudioLanguages { get; set; }
        public int AudioBitrate { get; set; }
        public int AudioSampleRate { get; set; }
        public TimeSpan Duration { get; set; }

        // Book properties
        public string Format { get; set; }
    }
}