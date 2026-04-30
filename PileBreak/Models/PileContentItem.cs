using System.Diagnostics.CodeAnalysis;

namespace PileBreak.Models
{
    public class PileContentItem : BaseContentItem
    {
        // "Game" or "Book"
        [NotNull]
        public string Category { get; set; }

        // Steam AppId や ISBN を格納（ランキング集計のキー）
        public string SourceId { get; set; }

        // UI表示用のアイコン名 (Material Iconなど)
        public string Icon { get; set; }
    }
}