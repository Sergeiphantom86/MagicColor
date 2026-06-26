using System;
using Game.SaveEditor;
using Menu.TutorialEditor;
using UnityEngine;

namespace PuzzleEditor.UI.LoadingScreen
{
    public class AdRewardController : MonoBehaviour
    {
        private const string RewardID = "after_puzzle_reward";

        [SerializeField]
        private OfferPanel _offerPanel;

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
                _offerPanel.Consent += ShowAd;
                _offerPanel.Cancelled += Complete;
            }

            _progressSaver.SubscribeADSReward(
                onRewardReceived: null,
                onAdOpened: null,
                onAdClosed: Complete,
                onAdError: Complete
            );
        }

        private void OnDisable()
        {
            if (_offerPanel != null)
            {
                _offerPanel.Consent -= ShowAd;
                _offerPanel.Cancelled -= Complete;
            }

            _progressSaver.UnsubscribeADSReward(
                onRewardReceived: null,
                onAdOpened: null,
                onAdClosed: Complete,
                onAdError: Complete
            );
        }

        public void ShowRewardAd(Action onComplete)
        {
            OnComplete = onComplete;

            if (_offerPanel == null)
                return;

            _offerPanel.TurnOn();
        }

        public void ShowAd()
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
}