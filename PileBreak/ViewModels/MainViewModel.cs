using PileBreak.Models;
using PileBreak.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PileBreak.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly DatabaseService _dbService;

        // 全データを保持するプライベートリスト
        private List<PileContentItem> _allItems = new();

        // ViewのCollectionViewがバインドする表示用リスト
        public ObservableCollection<PileContentItem> FilteredItems { get; } = new();

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ClearItemCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand GoToAddMenuCommand { get; }

        public MainViewModel(DatabaseService dbService)
        {
            _dbService = dbService;

            RefreshCommand = new Command(async () => await LoadItemsAsync());
            ClearItemCommand = new Command<PileContentItem>(async (item) => await ClearContentAsync(item));
            FilterCommand = new Command<string>((category) => ApplyFilter(category));

            GoToAddMenuCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("AddMenuPage");
            });
        }


        public async Task LoadItemsAsync()
        {
            IsRefreshing = true;

            // DBから「未解消」のアイテムを全件取得
            _allItems = await _dbService.GetItemsAsync(isCleared: false);

            // 画面に反映（一旦「すべて」を表示）
            ApplyFilter("All");

            IsRefreshing = false;
        }

        private void ApplyFilter(string category)
        {
            FilteredItems.Clear();

            var filtered = category == "All"
                ? _allItems
                : _allItems.Where(x => x.Category == category);

            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
            }
        }

        private async Task ClearContentAsync(PileContentItem item)
        {
            if (item == null) return;

            string comment = await Shell.Current.DisplayPromptAsync("解消完了！", "このゲームはどうでしたか？", "保存", "キャンセル", "一言メモ...", maxLength: 50);
            if (comment == null)
            {
                return;
            }
            string sanitized = Regex.Replace(comment, @"[^\w\s\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FAF（）！？」「：；、。,.!?]", "");
            item.IsCleared = 1;
            item.ClearedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            item.Comment = sanitized;
            await _dbService.UpdateItemAsync(item);

            // リストから削除して画面を更新
            _allItems.Remove(item);
            FilteredItems.Remove(item);
        }
    }
}