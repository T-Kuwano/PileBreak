using PileBreak.ViewModels;

namespace PileBreak.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // 画面が表示される（または戻ってくる）たびに実行
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // データの再読み込みを実行
            if (_viewModel != null)
            {
                await _viewModel.LoadItemsAsync();
            }
        }
    }
}