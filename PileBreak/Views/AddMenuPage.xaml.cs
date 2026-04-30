using PileBreak.ViewModels;

namespace PileBreak.Views
{
    public partial class AddMenuPage : ContentPage
    {
        public AddMenuPage(AddMenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}