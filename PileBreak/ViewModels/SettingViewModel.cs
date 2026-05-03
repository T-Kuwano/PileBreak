using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PileBreak.Services;

namespace PileBreak.ViewModels
{
    public partial class SettingViewModel : BindableObject
    {

        private readonly DatabaseService _dbService;

        private string _steamId = "";
        public string SteamId
        {
            get => _steamId;
            set
            {
                if (_steamId != value)
                {
                    _steamId = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("SavedSteamId", value);
                }
            }
        }

        private string _threshold = "";
        public string Threshold
        {
            get => _threshold;
            set
            {
                if (_threshold != value)
                {
                    _threshold = value;
                    OnPropertyChanged();
                    Preferences.Default.Set("SavedThreshold", value);
                }
            }
        }

        public record SettingChangedMessage(string Key, object Value);


        public SettingViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            SteamId = Preferences.Default.Get("SavedSteamId", string.Empty);
            Threshold = Preferences.Default.Get("SavedThreshold", string.Empty);

            // メッセージの受信登録（リスナー）
            WeakReferenceMessenger.Default.Register<SteamIdChangedMessage>(this, (r, m) =>
            {
                // m.Value に送信された ID が入っています
                // 必ずメインスレッドで UI プロパティを更新する
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.SteamId = m.Value;
                });
            });
        }

        /// <summary>
        /// データベースの全データを削除する
        /// </summary>
        [RelayCommand]
        private async Task ResetDatabase()
        {
            bool isConfirmed = false;
            if (Shell.Current == null) return;
            isConfirmed = await Shell.Current.DisplayAlert("警告", "全てのデータをリセットしますか？この操作は取り消せません。", "削除する", "キャンセル");
            if (isConfirmed)
            {
                await _dbService.ResetItemAsync();
                await Shell.Current.DisplayAlert("完了", "データベースを初期化しました。", "OK");
            }
        }
        [RelayCommand]
        private async Task LoginSteam()
        {
            try
            {
                string githubPagesUrl = "https://T-Kuwano.github.io/PileBreak/callback.html";

                string authUrl = "https://steamcommunity.com/openid/login?" +
                                 "openid.ns=http://specs.openid.net/auth/2.0&" +
                                 "openid.mode=checkid_setup&" +
                                 $"openid.return_to={githubPagesUrl}&" +
                                 $"openid.realm={githubPagesUrl}&" +
                                 "openid.identity=http://specs.openid.net/auth/2.0/identifier_select&" +
                                 "openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select";
                await Launcher.Default.OpenAsync(new Uri(authUrl));
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("エラー", "ブラウザの起動に失敗しました", "OK");
            }
        }
    }
}