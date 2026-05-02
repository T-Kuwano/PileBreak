using SQLite;
using System.Text.Json.Serialization;

namespace PileBreak.Models
{
    // Steam用
    public class SteamRawData
    {
        [PrimaryKey]
        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("img_icon_url")]
        public string ImgIconUrl { get; set; } = string.Empty;

        [JsonPropertyName("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonPropertyName("playtime_windows_forever")]
        public int PlaytimeWindowsForever { get; set; }

        [JsonPropertyName("rtime_last_played")]
        public int RtimeLastPlayed { get; set; }
    }

    // 書籍用
    public class BookRawData
    {
        [PrimaryKey]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
    }
}
