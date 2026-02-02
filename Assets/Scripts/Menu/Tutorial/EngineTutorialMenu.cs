using DG.Tweening;
using UnityEngine;

public class EngineTutorialMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _target;

    private Vector3 _originalScale;
    private Vector3 _targetScale;
    private Vector3 _originalPosition;
    private Sequence _animationSequence;
    private Sequence _scaleTween;
    private float _moveDuration;
    private float _moveDistance;
    private float _scaleMultiplier;
    private float _durationMultiplier;
    private bool _isSetPosition;
    private int _offset;

    private void Awake()
    {
        _offset = 5;
        _moveDuration = 1f;
        _moveDistance = 100;
        _scaleMultiplier = 2.5f;
        _durationMultiplier = 0.5f;
        _originalScale = transform.localScale;
        _targetScale = _originalScale * _scaleMultiplier;
    }

    public void StartAnimationMovements()
    {
        StopAnimation();

        _animationSequence = DOTween.Sequence();

        _animationSequence.Append(GetMove(transform.position.x - _moveDistance, _moveDuration));
        _animationSequence.Join(GetScaleReplaced(_targetScale));
        _animationSequence.Insert(GetScaleDuration(), GetScaleReplaced(transform.localScale));

        _animationSequence.SetLoops(-1, LoopType.Restart);

        ResumeAnimation();
    }

    public void StartAnimationClicks()
    {
        if (_isSetPosition) return;

        StopAnimation();

        transform.localScale = _originalScale;
        _originalPosition.x -= _offset;
        _originalPosition.y += _offset;
        transform.position = _originalPosition;

        GetInfiniteScaleLoop().Play();
        _isSetPosition = true;
    }

    public void StopAnimation()
    {
        if (_animationSequence != null && _animationSequence.IsActive())
        {
            _animationSequence.Kill();
        }
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }
    }

    public void ResumeAnimation()
    {
        _animationSequence?.Play();
        _scaleTween?.Play();
    }

    public void SetPosition(Vector3 position)
    {
        _originalPosition = position;
        position.x += _moveDistance * _durationMultiplier;
        position.y += _offset;

        transform.position = position;
    }

    private Sequence GetInfiniteScaleLoop()
    {
        _scaleTween = DOTween.Sequence();

        _scaleTween.Append(GetScaleReplaced(_targetScale));

        _scaleTween.SetLoops(-1, LoopType.Restart);

        return _scaleTween;
    }

    private Tween GetScaleReplaced(Vector3 scale)
    {
        return transform.DOScale(scale, GetScaleDuration())
           .SetEase(Ease.Linear);
    }

    private Tween GetMove(float targetPosition, float duration)
    {
        return transform.DOMoveX(targetPosition, duration)
            .SetEase(Ease.Linear);
    }

    private float GetScaleDuration()
    {
        return _moveDuration * _durationMultiplier;
    }

    private void OnDestroy()
    {
        _animationSequence?.Kill();
    }
}