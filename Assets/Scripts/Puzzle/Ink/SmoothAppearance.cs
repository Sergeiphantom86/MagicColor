using UnityEngine;
using DG.Tweening;
using System;

public class SmoothAppearance : MonoBehaviour
{
    private float _duration;
    private float _durationDeletion;
    private bool _useScale;
    private bool _disableOnStart;
    private Vector3 _originalScale;
    private SmoothMoveToTarget _smoothMoveToTarget;
    private Sequence _sequence;

    private void Awake()
    {
        _duration = 0.8f;
        _durationDeletion = 0.13f;
        _useScale = true;
        _disableOnStart = true;
        _originalScale = new Vector3(0.2f, 0.2f, 0.2f);
        _smoothMoveToTarget = GetComponent<SmoothMoveToTarget>();

        if (_disableOnStart)
        {
            if (_useScale) transform.localScale = Vector3.zero;
        }
    }

    private void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        CreateSizeChangeSequence(_originalScale, _duration, () => 
        _smoothMoveToTarget.BeginMovement());
    }

    public void Hide()
    {
        CreateSizeChangeSequence(Vector3.zero, _durationDeletion, () => 
        gameObject.SetActive(false));
    }

    private void CreateSizeChangeSequence(Vector3 scale, float duration, Action action = null)
    {
        if (_useScale)
        {
            _sequence = DOTween.Sequence();

            _sequence.Join(transform.DOScale(scale, duration).Play().SetEase(Ease.InElastic)).
                OnComplete(() => action?.Invoke());
        }
    }
}