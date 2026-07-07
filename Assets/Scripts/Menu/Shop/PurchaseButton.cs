using Menu.Tutorials.TutorialPuzzle;
using PuzzleEditor;
using PuzzleEditor.Audio;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wallets;
using Wallets.WalletEconomy;
using YG;

namespace Menu.Shop
{
    public class PurchaseButton : MonoBehaviour
    {
        private const string RewardID = "after_puzzle_reward";

        [SerializeField] private TextMeshProUGUI _paymentCoin;
        [SerializeField] private TextMeshProUGUI _paymentAdv;
        [SerializeField] private WalletAnimator _walletAnimator;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private Blocker _blocker;
        [SerializeField] private ParticleSystem _shine;
        [SerializeField] private Messager _hint;

        private Voiceover _voiceover;
        private Button _button;
        private IActivatable _activatable;
        private PaymentType _currentPaymentType;
        private long _result;
        private WaitForSeconds _waitForSeconds;

        private enum PaymentType
        {
            Coins,
            Ads,
        }

        public event Action Clicked;

        public event Action<long> CoinPurchased;

        public Button Button => _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _voiceover = GetComponent<Voiceover>();
            _waitForSeconds = new WaitForSeconds(_audioClip.length);
            _button.interactable = true;

            if (long.TryParse(_paymentCoin.text, out long result))
            {
                _result = result;
            }

            _activatable = _blocker;
            _activatable.Deactivate();

            if (_blocker != null && YG2.saves.IsUnlockAbilities == false)
            {
                _activatable.Activate();
                _button.enabled = false;
                _shine.Stop();
            }

            ChangeTypePayment();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnBuy);
        }

        private void OnEnable()
        {
            _walletAnimator.Finished += OnTurnOnButton;
        }

        private void OnDisable()
        {
            _walletAnimator.Finished -= OnTurnOnButton;
        }

        public void Click()
        {
            Clicked?.Invoke();

            _button.interactable = false;

            _voiceover.PlayOneShot(_audioClip);

            StartCoroutine(WaitTurnOnButton());
        }

        private IEnumerator WaitTurnOnButton()
        {
            yield return _waitForSeconds;

            _button.interactable = true;
        }

        private void ChangeTypePayment()
        {
            if (_paymentCoin == null)
            return;

            if (_paymentAdv == null)
            return;

            if (YG2.saves.CurrentCoin >= _result)
            {
                _currentPaymentType = PaymentType.Coins;

                _paymentCoin.gameObject.SetActive(true);
                _paymentAdv.gameObject.SetActive(false);
                return;
            }

            _currentPaymentType = PaymentType.Ads;

            _paymentCoin.gameObject.SetActive(false);
            _paymentAdv.gameObject.SetActive(true);
        }

        private void OnTurnOnButton()
        {
            _button.interactable = true;
        }

        private void OnBuy()
        {
            if (_currentPaymentType == PaymentType.Ads)
            {
                _button.interactable = false;

                if (YG2.nowRewardAdv == false && YG2.nowAdsShow == false)
                {
                    YG2.RewardedAdvShow(RewardID, null);
                    Click();
                }
                else
                {
                    _hint.TurnOn();
                }

                return;
            }

            if (_result <= 0)
                return;

            CoinPurchased?.Invoke(_result);
            ChangeTypePayment();
        }
    }
}