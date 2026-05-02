using Foundation;
using UIKit;

namespace PileBreak
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            // MAUIのApplicationクラスにURLを渡す
            Microsoft.Maui.Controls.Application.Current?.SendOnAppLinkRequestReceived(new Uri(url.AbsoluteString));
            return base.OpenUrl(application, url, options);
        }
    }
}
