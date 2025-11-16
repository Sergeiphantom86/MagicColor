using System.Collections;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    private int _rewardTutorial;
    private float _delay;
    private float _duration;
    private CoinWallet _coinWallet;
    private CrystalWallet _crystalWallet;
    private WaitForSeconds _waitForSeconds;
    private Coroutine _activationCoroutine;

    private void Awake()
    {
        _delay = 0.2f;
        _duration = 0.5f;
        _rewardTutorial = 5000;
        _waitForSeconds = new WaitForSeconds(_delay);
        _coinWallet = GetComponentInChildren<CoinWallet>(true);
        _crystalWallet = GetComponentInChildren<CrystalWallet>(true);

        TurnOffWallet();
    }

    private void Start()
    {
        EnableRewards();
    }

    private void OnDisable()
    {
        DisableRewardsImmediate();
    }

    private void EnableRewards()
    {
        if (_activationCoroutine != null)
        {
            StopCoroutine(_activationCoroutine);
        }
        _activationCoroutine = StartCoroutine(EnableRewardsWithDelay());
    }

    private void DisableRewardsImmediate()
    {
        if (_activationCoroutine != null)
        {
            StopCoroutine(_activationCoroutine);
            _activationCoroutine = null;
        }

        TurnOffWallet();
    }

    private void TurnOffWallet()
    {
        _coinWallet.gameObject.SetActive(false);
        _crystalWallet.gameObject.SetActive(false);
    }

    private IEnumerator EnableRewardsWithDelay()
    {
        yield return _waitForSeconds;

        TurnOnWallet(_coinWallet, true);

        yield return _waitForSeconds;

        TurnOnWallet(_crystalWallet, true);

        _activationCoroutine = null;
    }

    private void TurnOnWallet(Wallet wallet, bool isOn)
    {
        wallet.gameObject.SetActive(isOn);
        wallet.AddFunds(_rewardTutorial, _duration);
    }
}