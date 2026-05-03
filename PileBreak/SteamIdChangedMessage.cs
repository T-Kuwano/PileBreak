using CommunityToolkit.Mvvm.Messaging.Messages;

// string型の値を運ぶためのメッセージ
public class SteamIdChangedMessage : ValueChangedMessage<string>
{
    public SteamIdChangedMessage(string value) : base(value)
    {
    }
}