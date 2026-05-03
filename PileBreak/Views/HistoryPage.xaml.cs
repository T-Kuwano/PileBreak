using PileBreak.ViewModels;

namespace PileBreak.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;
    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

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