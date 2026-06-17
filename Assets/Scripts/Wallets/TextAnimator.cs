using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private bool _useSmoothAnimation = true;

    private Tween _balanceTween;
    private long _currentValue;
    private long _targetValue;
    private TextMeshProUGUI _textMeshPro;
    private NumberFormatter _numberFormatter;

    public long Value => _targetValue;

    private void Awake()
    {
        _numberFormatter = new NumberFormatter();
        _textMeshPro = GetComponent<TextMeshProUGUI>();

        if (_textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI == null");
        }
    }

    private void OnDestroy()
    {
        _balanceTween?.Kill();
    }

    public void AnimateToValue(long newValue, float customDuration = -1)
    {
        if (customDuration < 0)
            customDuration = _animationDuration;

        _balanceTween?.Kill();
        _targetValue = newValue;

        if (_useSmoothAnimation)
        {
            _balanceTween = DOTween.To(() => _currentValue,
                animatedValue =>
                {
                    _currentValue = animatedValue;
                    UpdateText();
                },
                _targetValue,
                customDuration)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            _currentValue = _targetValue;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (_textMeshPro != null)
        {
            _textMeshPro.text = $"{_numberFormatter.FormatNumber(_currentValue)}";
        }
    }
}