using SQLite;

namespace PileBreak.Models
{
    public abstract class BaseContentItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Title { get; set; }

        // 0: 未消化, 1: 消化済み
        public int IsCleared { get; set; } = 0;

        public string Comment { get; set; }

        public string CreatedDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public string ClearedDate { get; set; }
    }
}