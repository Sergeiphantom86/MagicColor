using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SmoothAppearance : MonoBehaviour
{
    private float _duration;
    private float _durationDeletion;
    private bool _useScale;
    private bool _disableOnStart;
    private Vector3 _originalScale;
    private SmoothMoveToTarget _smoothMoveToTarget;

    private void Awake()
    {
        _duration = 0.8f;
        _durationDeletion = 0.13f;
        _useScale = true;
        _disableOnStart = true;
        _originalScale = transform.localScale;
        _smoothMoveToTarget = GetComponent<SmoothMoveToTarget>();

        if (_disableOnStart)
        {
            if (_useScale) transform.localScale = Vector3.zero;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        CreateSizeChangeSequence(_originalScale, _duration).Play();

        StartCoroutine(DelayMovementStart());
    }

    public void Hide()
    {
        Sequence disappearSequence = CreateSizeChangeSequence(Vector3.zero, _durationDeletion);

        disappearSequence.OnComplete(() =>
        gameObject.SetActive(false));

        disappearSequence.Play();
    }

    private Sequence CreateSizeChangeSequence(Vector3 scale, float duration)
    {
        Sequence sequence = DOTween.Sequence();

        if (_useScale)
        {
            sequence.Join(transform.DOScale(scale, duration).SetEase(Ease.InElastic));
        }

        return sequence;
    }

    private IEnumerator DelayMovementStart()
    {
        yield return new WaitForSeconds(0.3f);

        _smoothMoveToTarget.BeginMovement();
    }
}