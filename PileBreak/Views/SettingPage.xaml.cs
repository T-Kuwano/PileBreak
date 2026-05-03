using PileBreak.ViewModels;

namespace PileBreak.Views;

public partial class SettingPage : ContentPage
{

    private readonly SettingViewModel _viewModel;

    public SettingPage(SettingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void OnBackgroundTapped(object sender, EventArgs e)
    {
        SteamIdEntry.Unfocus();
        ThresholdEntry.Unfocus();
    }

}