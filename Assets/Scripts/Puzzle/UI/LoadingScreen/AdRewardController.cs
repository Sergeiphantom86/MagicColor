using System;
using UnityEngine;
using YG;

public class AdRewardController : MonoBehaviour
{
    private const string RewardID = "after_puzzle_reward";

    [SerializeField] private OfferPanel _offerPanel;

    private Action OnComplete;

    private void OnEnable()
    {
        _offerPanel.OnConsent += ShowAd;
        _offerPanel.OnCancelled += Complete;

        YG2.onCloseRewardedAdv += Complete;
        YG2.onErrorRewardedAdv += Complete;
    }

    private void OnDisable()
    {
        _offerPanel.OnConsent -= ShowAd;
        _offerPanel.OnCancelled -= Complete;

        YG2.onCloseRewardedAdv -= Complete;
        YG2.onErrorRewardedAdv -= Complete;
    }

    public void ShowRewardAd(Action onComplete)
    {
        OnComplete = onComplete;
        _offerPanel.TurnOn();
    }

    private void ShowAd()
    {
        if (YG2.nowRewardAdv == false && YG2.nowAdsShow == false)
        {
            YG2.RewardedAdvShow(RewardID, null);
        }
        else
        {
            Complete();
        }
    }

    private void Complete()
    {
        OnComplete?.Invoke();
    }
}