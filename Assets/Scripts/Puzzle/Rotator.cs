using DG.Tweening;
using System;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _duration;
    [SerializeField] private float _targetAngleX;
    [SerializeField] private RotateMode _rotateMode;
    [SerializeField] private Ease _easeType;

    [Header("Movement Settings")]
    [SerializeField] private float _targetY;
    [SerializeField] private bool _useLocalMovement = true;

    [Header("Loop Settings")]
    [SerializeField] private int _loops;
    [SerializeField] private LoopType _loopType;

    private Tween _rotationTween;
    private float _returnAngle;
    private float _startY;

    public event Action OnRotated;

    private void Awake()
    {
        _returnAngle = 90;
        _startY = _useLocalMovement ? transform.localPosition.y : transform.position.y;
    }

    public void StartRotation()
    {
        SetTarget(_targetAngleX, _targetY);
    }

    public void Return()
    {
        SetTarget(_returnAngle, _startY);
    }

    private void SetTarget(float targetAngleX, float targetY)
    {
        _rotationTween?.Kill();

        Sequence sequence = DOTween.Sequence();

        sequence.Join(GetTweenRotation(targetAngleX));
        sequence.Join(GetTweenMove(targetY));

        sequence.SetLoops(_loops, _loopType);
        sequence.OnComplete(() => OnRotated?.Invoke());

        _rotationTween = sequence;
    }

    private void OnDestroy()
    {
        _rotationTween?.Kill();
    }

    private Tween GetTweenMove(float targetY)
    {
        Tween moveTween = _useLocalMovement ?
           transform.DOLocalMoveY(targetY, _duration) :
           transform.DOMoveY(targetY, _duration);

        return moveTween.SetEase(_easeType);
    }

    private Tween GetTweenRotation(float targetAngleX)
    {
        return transform.DORotate(
            new Vector3(targetAngleX, 0, 0),
            _duration,
            _rotateMode
        ).SetEase(_easeType);
    }
}