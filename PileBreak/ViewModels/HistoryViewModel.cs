using PileBreak.Models;
using PileBreak.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PileBreak.ViewModels
{
    public class HistoryViewModel : BindableObject
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
        public ICommand FilterCommand { get; }

        public HistoryViewModel(DatabaseService dbService)
        {
            _dbService = dbService;

            RefreshCommand = new Command(async () => await LoadItemsAsync());
            FilterCommand = new Command<string>((category) => ApplyFilter(category));
        }


        public async Task LoadItemsAsync()
        {
            IsRefreshing = true;

            // DBから「未解消」のアイテムを全件取得
            _allItems = await _dbService.GetItemsAsync(isCleared: true);

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
    }
}