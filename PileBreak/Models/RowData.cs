using SQLite;

namespace PileBreak.Models
{
    // Steam用
    public class SteamRawData
    {
        [PrimaryKey]
        public string AppId { get; set; }
        public string Name { get; set; }
        public string HeaderImage { get; set; }
        public int Playtime { get; set; }
        public string LastUpdated { get; set; }
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
