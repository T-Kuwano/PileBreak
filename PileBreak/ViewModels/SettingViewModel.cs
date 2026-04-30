using System.Diagnostics;
using System.Windows.Input;

namespace PileBreak.ViewModels
{
    public class SettingViewModel : BindableObject
    {
        private string _steamId;

        // --- プロパティ ---

        public string SteamId
        {
            get => _steamId;
            set
            {
                if (_steamId != value)
                {
                    _steamId = value;
                    OnPropertyChanged();
                    // setterの中で保存することで、手入力でもログイン取得でも自動保存される
                    Preferences.Default.Set("SavedSteamId", value);
                }
            }
        }

        // --- コマンド ---

        public ICommand LoginSteamCommand { get; }
        public ICommand ResetDatabaseCommand { get; }

        // --- コンストラクタ ---

        public SettingViewModel()
        {
            // 起動時に Preferences から現在の保存値を読み込む
            // 保存されていなければ string.Empty が返る
            SteamId = Preferences.Default.Get("SavedSteamId", string.Empty);

            LoginSteamCommand = new Command(async () => await OnLoginSteam());
            ResetDatabaseCommand = new Command(async () => await OnResetDatabase());
        }

        // --- メソッド (ロジック) ---

        /// <summary>
        /// Steam OpenID を利用して ID を自動取得する
        /// </summary>
        private async Task OnLoginSteam()
        {
            try
            {
                // アプリに戻るためのカスタムURL（これが重要！）
                string callbackUrl = "pilebreak://";

                // 1. Steam ログイン用 URL の組み立て
                // return_to と realm をアプリのスキームに変更します
                string authUrl = "https://steamcommunity.com/openid/login?" +
                                 "openid.ns=http://specs.openid.net/auth/2.0&" +
                                 "openid.mode=checkid_setup&" +
                                 $"openid.return_to={callbackUrl}&" + // 修正箇所
                                 $"openid.realm={callbackUrl}&" +      // 修正箇所
                                 "openid.identity=http://specs.openid.net/auth/2.0/identifier_select&" +
                                 "openid.claimed_id=http://specs.openid.net/auth/2.0/identifier_select";

                // 2. ブラウザを起動し、アプリのカスタムURLスキームへのリダイレクトを待つ
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(authUrl),
                    new Uri(callbackUrl));

                // 3. 戻ってきた情報から ID を抽出
                if (authResult?.Properties.TryGetValue("openid.claimed_id", out var claimedId) == true)
                {
                    var extractedId = claimedId.Split('/').Last();
                    SteamId = extractedId; // これで Preferences にも保存される

                    await App.Current.MainPage.DisplayAlert("成功", "Steam ID を取得しました", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login failed: {ex.Message}");
            }
        }

        /// <summary>
        /// データベースの全データを削除する
        /// </summary>
        private async Task OnResetDatabase()
        {
            bool isConfirmed = await App.Current.MainPage.DisplayAlert(
                "警告",
                "全てのデータをリセットしますか？この操作は取り消せません。",
                "削除する",
                "キャンセル");

            if (isConfirmed)
            {
                // ここに DatabaseService の削除メソッドを呼び出す処理を書く
                // 例: await _databaseService.DeleteAllGamesAsync();

                await App.Current.MainPage.DisplayAlert("完了", "データベースを初期化しました。", "OK");
            }
        }
    }
}