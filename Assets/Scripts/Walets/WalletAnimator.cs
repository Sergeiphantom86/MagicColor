using TMPro;
using DG.Tweening;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Wallet))]
public class WalletAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private Wallet _wallet;
    private long _displayedBalance;
    private Tween _balanceTween;
    private float _animationDuration = 0.5f;

    private void Awake()
    {
        _wallet = GetComponent<Wallet>();
        _displayedBalance = _wallet.Balance;
        UpdateBalanceText();

        _wallet.OnBalanceChanged += HandleBalanceChanged;
    }

    private void HandleBalanceChanged(long newBalance, string name)
    {
        StartCoroutine(WaitEndAnimation(newBalance));
    }

    private void UpdateBalanceText()
    {
        _textMeshPro.text = NumberFormatter.FormatNumber(_displayedBalance);
    }

    private void OnDestroy()
    {
        _wallet.OnBalanceChanged -= HandleBalanceChanged;
        _balanceTween?.Kill();
    }

    private IEnumerator WaitEndAnimation(long newBalance)
    {
        yield return new WaitForSeconds(_wallet.Duration);

        _balanceTween?.Kill();

        _balanceTween = DOTween.To(() =>
        _displayedBalance, animatedValue =>
        {
            _displayedBalance = animatedValue;
            UpdateBalanceText();
        }, newBalance, _animationDuration
        ).SetEase(Ease.OutQuad);
    }
}