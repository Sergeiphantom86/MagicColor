using System;
using DG.Tweening;
using PuzzleEditor.SoundEditor;
using TMPro;
using UnityEngine;

namespace Wallets
{
    [RequireComponent(typeof(Wallet), typeof(TextMeshProUGUI), typeof(Voiceover))]
    public class WalletAnimator : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _audioClip;

        private float _soundTimer;
        private long _displayedBalance;
        private float _animationDuration;
        private Wallet _wallet;
        private Tween _balanceTween;
        private Voiceover _voiceover;
        private TextMeshProUGUI _textMeshPro;
        private NumberFormatter _numberFormatter;

        public event Action Finished;

        private void Awake()
        {
            _wallet = GetComponent<Wallet>();
            _voiceover = GetComponent<Voiceover>();
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            _numberFormatter = new NumberFormatter();

            _displayedBalance = _wallet.Balance;
            _animationDuration = 0.5f;

            if (_wallet == null)
            {
                Debug.Log("Wallet == null");
                return;
            }

            if (_textMeshPro == null)
            {
                Debug.Log("TextMeshProUGUI == null");
                return;
            }

            UpdateBalanceText();
        }

        private void OnEnable()
        {
            _wallet.OnBalanceChanged += HandleBalanceChanged;
        }

        private void OnDestroy()
        {
            _wallet.OnBalanceChanged -= HandleBalanceChanged;

            _balanceTween?.Kill();
        }

        private void HandleBalanceChanged(long newBalance, string name)
        {
            HandleBalanceChanged(newBalance);
        }

        private void UpdateBalanceText()
        {
            _textMeshPro.text = _numberFormatter.FormatNumber(_displayedBalance);
        }

        private void HandleBalanceChanged(long newBalance)
        {
            _balanceTween?.Kill();

            _balanceTween = DOTween
                .To(
                    () => _displayedBalance,
                    balance =>
                    {
                        _soundTimer += Time.unscaledDeltaTime;

                        if (_soundTimer >= 0.05f)
                        {
                            _voiceover.PlayOneShot(_audioClip);
                            _soundTimer = 0f;
                        }

                        _displayedBalance = balance;
                        UpdateBalanceText();
                    },
                    newBalance,
                    _animationDuration
                )
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _soundTimer = 0f;
                    Finished?.Invoke();
                });
        }
    }
}