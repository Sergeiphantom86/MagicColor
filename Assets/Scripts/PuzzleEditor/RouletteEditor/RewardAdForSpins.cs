using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace PuzzleEditor.RouletteEditor
{
    [RequireComponent(typeof(Button))]

    public class RewardAdForSpins : MonoBehaviour
    {
        private readonly string _rewardID = "add_spins";

        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private Button _button;

        public event Action SpinsAdded;

        private void Awake()
        {
            _button = GetComponent<Button>();

            if (_button == null)
            {
                Debug.LogError("Button �� ��������!!!");
                return;
            }

            _button.onClick.AddListener(OnShowRewardedAd);

            YG2.onRewardAdv += OnRewardReceived;
            YG2.onOpenRewardedAdv += OnAdOpened;
            YG2.onCloseRewardedAdv += OnAdClosed;
            YG2.onErrorRewardedAdv += OnAdError;
        }

        private void OnEnable()
        {
            OnAdError();
        }

        private void OnDisable()
        {
            _button.interactable = false;
        }

        private void OnShowRewardedAd()
        {
            _button.interactable = false;

            YG2.RewardedAdvShow(_rewardID);
        }

        private void OnRewardReceived(string id)
        {
            if (id == _rewardID)
            {
                for (int i = 0; i < ParseTextToInt(); i++)
                {
                    SpinsAdded?.Invoke();

                    gameObject.SetActive(false);
                }
            }
        }

        public int ParseTextToInt()
        {
            string numericText = new string(
            _textMeshPro.text.Where(c => char.IsDigit(c) || c == '-').ToArray()
            );

            return int.TryParse(numericText, out int result) ? result : 0;
        }

        private void OnAdOpened()
        {
            Time.timeScale = 0f;
        }

        private void OnAdClosed()
        {
            Time.timeScale = 1f;
        }

        private void OnAdError()
        {
            _button.interactable = true;
        }

        private void OnDestroy()
        {
            YG2.onRewardAdv -= OnRewardReceived;
            YG2.onOpenRewardedAdv -= OnAdOpened;
            YG2.onCloseRewardedAdv -= OnAdClosed;
            YG2.onErrorRewardedAdv -= OnAdError;

            if (_button != null)
                _button.onClick.RemoveListener(OnShowRewardedAd);
        }
    }
}