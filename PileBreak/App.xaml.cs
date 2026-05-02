using CommunityToolkit.Mvvm.Messaging;
using System.Web;

namespace PileBreak
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            // スキームが pilebreak かつ ホストが callback であることを確認
            if (uri.Scheme == "pilebreak")
            {
                // クエリパラメータから id を取得
                var query = HttpUtility.ParseQueryString(uri.Query);
                var steamId = query.Get("id");

                if (!string.IsNullOrEmpty(steamId))
                {
                    // 1. 保存
                    Preferences.Default.Set("SavedSteamId", steamId);

                    // 2. ViewModelへ直接通知 (WeakReferenceMessengerを使用)
                    // AppShellを経由せず、ここで飛ばしてしまいます
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        WeakReferenceMessenger.Default.Send(steamId);
                    });
                }
            }
        }
    }
}