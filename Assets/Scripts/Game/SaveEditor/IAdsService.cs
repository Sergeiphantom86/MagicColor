using System;
namespace Game.SaveEditor
{

public interface IAdsService
{
    public bool CanShowAd();

    public void RewardedAdvShow(string rewardID, Action action = null);

    public void SubscribeADSReward(
        Action<string> onRewardReceived,
        Action onAdOpened,
        Action onAdClosed,
        Action onAdError);

    public void UnsubscribeADSReward(
        Action<string> onRewardReceived,
        Action onAdOpened,
        Action onAdClosed,
        Action onAdError);
}
}