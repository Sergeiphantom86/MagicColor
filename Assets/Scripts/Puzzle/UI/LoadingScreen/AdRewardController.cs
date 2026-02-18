using System;
using UnityEngine;

public class AdRewardController : MonoBehaviour
{
    private const string RewardID = "after_puzzle_reward";

    [SerializeField] private OfferPanel _offerPanel;

    private IProgressSaver _progressSaver;

    private Action OnComplete;

    private void Awake()
    {
        _progressSaver = new ProgressSaver();
    }

    private void OnEnable()
    {
        if (_offerPanel != null)
        {
            _offerPanel.OnConsent += ShowAd;
            _offerPanel.OnCancelled += Complete;
        }

        _progressSaver.SubscribeADSReward(
            onRewardReceived: null, 
            onAdOpened: null, 
            onAdClosed: Complete, 
            onAdError: Complete);
    }

    private void OnDisable()
    {
        if (_offerPanel != null)
        {
            _offerPanel.OnConsent -= ShowAd;
            _offerPanel.OnCancelled -= Complete;
        }

        _progressSaver.UnsubscribeADSReward(
            onRewardReceived: null,
            onAdOpened: null,
            onAdClosed: Complete,
            onAdError: Complete);
    }

    public void ShowRewardAd(Action onComplete)
    {
        OnComplete = onComplete;
        _offerPanel.TurnOn();
    }

    private void ShowAd()
    {
        if (_progressSaver.CanShowAd())
        {
            _progressSaver.RewardedAdvShow(RewardID, null);
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