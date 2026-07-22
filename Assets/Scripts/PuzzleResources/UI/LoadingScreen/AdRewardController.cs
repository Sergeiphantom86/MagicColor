using Menu.Tutorials;
using System;
using UnityEngine;
using YG;

namespace PuzzleResources.UI.LoadingScreen
{
    public class AdRewardController : MonoBehaviour
    {
        private const string RewardID = "after_puzzle_reward";

        [SerializeField] private OfferPanel _offerPanel;

        private Action _onComplete;

        private void OnEnable()
        {
            if (_offerPanel != null)
            {
                _offerPanel.Consented += ShowAd;
                _offerPanel.Cancelled += Complete;
            }

            YG2.onCloseRewardedAdv += Complete;
            YG2.onErrorRewardedAdv += Complete;
        }

        private void OnDisable()
        {
            if (_offerPanel != null)
            {
                _offerPanel.Consented -= ShowAd;
                _offerPanel.Cancelled -= Complete;
            }

            YG2.onCloseRewardedAdv -= Complete;
            YG2.onErrorRewardedAdv -= Complete;
        }

        public void ShowRewardAd(Action onComplete)
        {
            _onComplete = onComplete;

            if (_offerPanel == null)
                return;

            _offerPanel.TurnOn();
        }

        public void ShowAd()
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
            _onComplete?.Invoke();
        }
    }
}