using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PanelFader : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 1;

    private CanvasGroup _canvasGroup;
    private Tween _currentTween;
    private float _blackout;

    private void Awake()
    {
        _blackout = 1;
    }

    private void OnDisable()
    {
        _currentTween?.Kill();
    }

    public void FadeIn(Action onComplete = null)
    {
        Fade(_blackout, true).
            OnComplete(() => onComplete?.Invoke());
    }

    public void FadeOut(Action onComplete = null)
    {
        Fade(0f, false).
            OnComplete(() =>
            onComplete?.Invoke());
    }

    public Tween Fade(float targetAlpha, bool isInteractable)
    {
        _currentTween?.Kill();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _currentTween = _canvasGroup.DOFade(targetAlpha, _fadeDuration)
          .SetLink(gameObject)
          .OnStart(() =>
          {
              if (_canvasGroup != null)
              {
                  _canvasGroup.interactable = isInteractable;
                  _canvasGroup.blocksRaycasts = isInteractable;
              }
          });
        return _currentTween;
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}