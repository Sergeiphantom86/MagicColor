using System;

public interface IYGEvents
{
    public void SubscribeSDKData(Action onYGDataLoaded);
    public void UnsubscribeSDKData(Action onYGDataLoaded);
    public void SubscribeSwitchLang(Action<string> onLanguageChanged);
    public void UnsubscribeSwitchLang(Action<string> onLanguageChanged);
}