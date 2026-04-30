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
    }
}
