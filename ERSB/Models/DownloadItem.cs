using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ERSB.Models
{
    public partial class DownloadItem
    {
        #region DownloadProgress
        [JsonProperty("guid")]
        public Guid Guuid { get; set; }

        [JsonProperty("receivedBytes")]
        public long ReceivedBytes { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("totalBytes")]
        public long TotalBytes { get; set; }

        #endregion

        #region DownloadBegin
        [JsonProperty("frameId")]
        public string FrameId { get; set; }

        [JsonProperty("suggestedFilename")]
        public string SuggestedFilename { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
        #endregion
    }
    public partial class DownloadItem
    {
        public static DownloadItem FromJson(string json) => JsonConvert.DeserializeObject<DownloadItem>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this DownloadItem self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
