using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Blinker : MonoBehaviour, IActivatable
{
    [SerializeField] private float _blinkSpeed = 1f;
    [SerializeField] private float _minAlpha = 0f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private bool _blinkOnStart = true;
    [SerializeField] private Button _button;

    private Image _targetImage;
    private Tween _blinkTween;
    private Color _originalColor;

    public event Action OnCompleted;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();

        if (_targetImage == null)
        {
            Debug.LogError("Blinker: Нет компонента Image на объекте " + gameObject.name);
            enabled = false;
            return;
        }
        _originalColor = _targetImage.color;

        Deactivate();
    }

    private void Start()
    {
        if (_blinkOnStart)
            Play();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(Stop);
    }

    private void OnDisable()
    {
        _blinkTween?.Kill();
    }

    public void Play()
    {
        Color startColor = _originalColor;
        startColor.a = _maxAlpha;
        _targetImage.color = startColor;

        _blinkTween = _targetImage.DOFade(_minAlpha, _blinkSpeed / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    public void Stop()
    {
        OnCompleted?.Invoke();

        if (_blinkTween != null && _blinkTween.IsActive())
        {
            _blinkTween.Kill();
            _blinkTween = null;
        }

        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}