using DG.Tweening;
using UnityEngine;
using System;

public class DialogueAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform _dialoguePanel;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Vector3 _originalScale;
    private float _showDuration;
    private float _hideDuration;

    private void Awake()
    {
        _showDuration = 0.5f;
        _hideDuration = 0.3f;
        _originalScale = _dialoguePanel.localScale;

        SetActive(false);
    }

    public void Show()
    {
        SetActive(true);
        AnimateShow();
    }

    public void Hide()
    {
        AnimateHide(() => SetActive(false));
    }

    private void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void AnimateShow()
    {
        _dialoguePanel.localScale = Vector3.zero;
        _canvasGroup.alpha = 0;

        InitializeSequence(1, _showDuration, _originalScale).SetEase(Ease.OutBack);
    }

    private void AnimateHide(Action onComplete)
    {
        InitializeSequence(0, _hideDuration, Vector3.zero)
            .SetEase(Ease.InBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    private Sequence InitializeSequence(float target, float duration, Vector3 scale)
    {
        return DOTween.Sequence()
           .Append(_canvasGroup.DOFade(target, duration))
           .Join(_dialoguePanel.DOScale(scale, duration).SetEase(Ease.OutBack));
    }
}