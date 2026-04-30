using PileBreak.Models;
using PileBreak.Services;
using System.Collections.ObjectModel;
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
        public ICommand GoToSettingCommand { get; }

        public MainViewModel(DatabaseService dbService)
        {
            _dbService = dbService;

            // 引っ張って更新する処理
            RefreshCommand = new Command(async () => await LoadItemsAsync());

            // アイテムを解消する処理
            ClearItemCommand = new Command<PileContentItem>(async (item) => await ClearContentAsync(item));

            // カテゴリで絞り込む処理
            FilterCommand = new Command<string>((category) => ApplyFilter(category));

            GoToAddMenuCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("AddMenuPage");
            });

            GoToSettingCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("SettingPage");
            });
        }

        // --- ここが MainPage.xaml.cs から呼ばれるメソッド ---
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

            item.IsCleared = 1;
            item.ClearedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            item.Comment = "解消済み";

            await _dbService.SaveItemAsync(item);

            // リストから削除して画面を更新
            _allItems.Remove(item);
            FilteredItems.Remove(item);
        }
    }
}