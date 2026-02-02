using TMPro;
using DG.Tweening;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Wallet))]
public class WalletAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private Wallet _wallet;
    private Tween _balanceTween;
    private long _displayedBalance;
    private float _animationDuration;
    private WaitForSeconds _waitForSeconds;
    private NumberFormatter _numberFormatter;

    private void Awake()
    {
        _animationDuration = 0.5f;
        _wallet = GetComponent<Wallet>();
        _displayedBalance = _wallet.Balance;
        _waitForSeconds = new WaitForSeconds(_wallet.Duration);
        _numberFormatter = new NumberFormatter();

        UpdateBalanceText();

        _wallet.OnBalanceChanged += HandleBalanceChanged;
    }

    private void OnDestroy()
    {
        _wallet.OnBalanceChanged -= HandleBalanceChanged;
        _balanceTween?.Kill();
    }

    private void HandleBalanceChanged(long newBalance, string name)
    {
        StartCoroutine(WaitEndAnimation(newBalance));
    }

    private void UpdateBalanceText()
    {
        _textMeshPro.text = _numberFormatter.FormatNumber(_displayedBalance);
    }

    private IEnumerator WaitEndAnimation(long newBalance)
    {
        yield return _waitForSeconds;

        _balanceTween?.Kill();

        _balanceTween = DOTween.To(() =>
        _displayedBalance, animatedValue =>
        {
            _displayedBalance = animatedValue;
            UpdateBalanceText();
        }, 
        newBalance, _animationDuration).
        SetEase(Ease.OutQuad);
    }
}