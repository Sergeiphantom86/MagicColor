using DG.Tweening;
using UnityEngine;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class PanelFader : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 1f;

    private CanvasGroup _canvasGroup;
    private Tween _currentTween;
    private float _blackout;

    public float FadeDuration => _fadeDuration;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _blackout = 1;
    }

    public void FadeIn(Action onComplete = null)
    {
        Fade(_blackout, true).
            OnComplete(() => onComplete?.Invoke());
    }

    public void FadeOut(Action onComplete = null)
    {
        Fade(0f, false).
            OnComplete(() => onComplete?.Invoke());
    }

    public Tween Fade(float targetAlpha, bool isInteractable)
    {
        _currentTween?.Kill();

        _currentTween = _canvasGroup.DOFade(targetAlpha, _fadeDuration)
            .OnStart(() =>
            {
                _canvasGroup.interactable = isInteractable;
                _canvasGroup.blocksRaycasts = isInteractable;
            });

         return _currentTween;
    }

    public void RestoreValues(float blackout, bool isOn)
    {
        _canvasGroup.alpha = blackout;
        _canvasGroup.interactable = isOn;
        _canvasGroup.blocksRaycasts = isOn;
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}