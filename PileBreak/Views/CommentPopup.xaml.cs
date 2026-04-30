using CommunityToolkit.Maui.Views;

namespace PileBreak.Views;

public partial class CommentPopup : Popup
{
    public CommentPopup()
    {
        InitializeComponent();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        Close(); // ポップアップを閉じる
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // nullを返して「何もしなかった」ことを伝える
    }
}