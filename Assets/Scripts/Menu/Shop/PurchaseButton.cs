using Game.SaveEditor;
using Menu.TutorialEditor.TutorialPuzzle;
using PuzzleEditor;
using PuzzleEditor.SoundEditor;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wallets;
using Wallets.WalletEditor;
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
    private IProgressSaver _progressSaver;
    private IActivatable _activatable;
    private PaymentType _currentPaymentType;
    private long _result;

    public event Action Clicked;

    public event Action<long> CoinPurchased;

    public Button Button => _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _voiceover = GetComponent<Voiceover>();
        _progressSaver = new ProgressSaver();
        _button.interactable = true;

        if (long.TryParse(_paymentCoin.text, out long result))
        {
            _result = result;
        }

        _activatable = _blocker;
        _activatable.Deactivate();

        if (_blocker != null && _progressSaver.Saves.IsUnlockAbilities == false)
        {
            _activatable.Activate();
            _button.enabled = false;
            _shine.Stop();
        }

        TryChangeTypePayment();
    }

    private void Start()
    {
        _button.onClick.AddListener(Buy);
    }

    private void OnEnable()
    {
        _walletAnimator.Finished += TurnOnButton;
    }

    private void OnDisable()
    {
        _walletAnimator.Finished -= TurnOnButton;
    }

    public void Click()
    {
        Clicked?.Invoke();

        _button.interactable = false;

        _voiceover.PlayOneShot(_audioClip);

        StartCoroutine(WaitTurnOnButton(_audioClip.length));
    }

    private void Buy()
    {
        if (_currentPaymentType == PaymentType.Ads)
        {
            _button.interactable = false;

            if (_progressSaver.CanShowAd())
            {
                _progressSaver.RewardedAdvShow(RewardID, null);
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
        TryChangeTypePayment();
    }

    private void TurnOnButton()
    {
        StartCoroutine(WaitTurnOnButton());
    }

    private IEnumerator WaitTurnOnButton(float delay = 0)
    {
        yield return new WaitForSecondsRealtime(delay);

        _button.interactable = true;
    }

    private void TryChangeTypePayment()
    {
        if (_progressSaver == null)
            return;

        if (_paymentCoin == null)
            return;

        if (_paymentAdv == null)
            return;

        if (_progressSaver.Saves.CurrentCoin >= _result)
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
}

public enum PaymentType
{
    Coins,
    Ads,
}
}