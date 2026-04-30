using PileBreak.Services;
using System.Windows.Input;

namespace PileBreak.ViewModels
{
    public class AddMenuViewModel : BindableObject
    {
        private readonly SteamApiService _steamService;

        public ICommand SyncSteamCommand { get; }
        public ICommand GoToBookSearchCommand { get; }
        public ICommand GoToManualAddCommand { get; }

        public AddMenuViewModel(SteamApiService steamService)
        {
            _steamService = steamService;

            SyncSteamCommand = new Command(async () =>
            {
                // インジケーターを出して処理
                await _steamService.FetchAndSyncSteamGamesAsync();
                await Shell.Current.DisplayAlert("完了", "Steamのゲームを取り込みました", "OK");
                await Shell.Current.GoToAsync(".."); // 前の画面（MainPage）に戻る
            });

            // 未実装のボタン用
            GoToBookSearchCommand = new Command(() => Shell.Current.DisplayAlert("予定", "今後のアップデートで実装予定です", "OK"));
            GoToManualAddCommand = new Command(() => Shell.Current.DisplayAlert("予定", "今後のアップデートで実装予定です", "OK"));
        }
    }
}