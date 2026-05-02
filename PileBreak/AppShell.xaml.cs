using CommunityToolkit.Mvvm.Messaging;
using System.Web;

namespace PileBreak
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("AddMenuPage", typeof(Views.AddMenuPage));
            Routing.RegisterRoute("SettingPage", typeof(Views.SettingPage));
        }

        /// <summary>
        /// アプリ内での遷移が発生したときに呼ばれる
        /// ブラウザからの戻り（カスタムURLスキーム）もここでキャッチする
        /// </summary>
        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            // 遷移先のURL文字列を取得
            var targetLocation = args.Target.Location.OriginalString;

            // URLに Steam ID が含まれているか確認
            if (targetLocation.Contains("id="))
            {
                try
                {
                    // Uriオブジェクトとして解析しやすくするためダミーのホストを付与
                    var uri = new Uri("http://localhost/" + targetLocation);
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    var steamId = query.Get("id");

                    if (!string.IsNullOrEmpty(steamId))
                    {
                        // 1. 永続保存 (次にアプリを開いた時用)
                        Preferences.Default.Set("SavedSteamId", steamId);

                        // 2. 現在開いている ViewModel へ通知 (リアルタイム更新用)
                        // WeakReferenceMessenger を使用
                        WeakReferenceMessenger.Default.Send(steamId);

                        // 3. ユーザーへの通知（UIスレッドで実行）
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await DisplayAlert("連携成功", $"Steam ID: {steamId} を取得しました", "OK");
                        });
                    }
                }
                catch (Exception ex)
                {
                    // 解析エラー時の処理
                    System.Diagnostics.Debug.WriteLine($"URL解析エラー: {ex.Message}");
                }
            }
        }
    }
}
