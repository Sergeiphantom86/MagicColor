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

    private Tween _rotationTween;
    private float _targetX;
    private float _targetY;
    private float _targetZ;

    public event Action OnRotated;

    private void OnDestroy()
    {
        _rotationTween?.Kill();
    }

    public void StartRotation()
    {
        _rotationTween?.Kill();

        _rotationTween = GetSequence();

        //SetPositionPuzzle(0.25f, 16, 1);
    }

    public void SetPositionPuzzle(float targetX, float targetY, float targetZ)
    {
        _targetX = targetX;
        _targetY = targetY;
        _targetZ = targetZ;
    }

    private Sequence GetSequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(GetTweenRotation(_targetAngleX));

        //sequence.Join(GetTweenMoveX(_targetX));
        //sequence.Join(GetTweenMoveY(_targetY));
        //sequence.Join(GetTweenMoveZ(_targetZ));

        sequence.Join(GetTweenMove(_targetX, _targetY, _targetZ));

        sequence.OnComplete(() => 
        OnRotated?.Invoke());

        return sequence;
    }

    private Tween GetTweenMove(float targetX, float targetY, float targetZ)
    {
        return transform.DOLocalMove(new Vector3(targetX, targetY, _targetZ), _duration);
    }

    private Tween GetTweenMoveY(float targetY)
    {
        return transform.DOLocalMoveY(targetY, _duration);
    }

    private Tween GetTweenMoveX(float targetX)
    {
        return transform.DOLocalMoveX(targetX, _duration);
    }

    private Tween GetTweenMoveZ(float targetZ)
    {
        return transform.DOLocalMoveZ(targetZ, _duration);
    }

    private Tween GetTweenRotation(float targetAngleX)
    {
        return transform.DORotate(GetTargetAngleX(targetAngleX), _duration,_rotateMode)
            .SetEase(_easeType);
    }

    private Vector3 GetTargetAngleX(float targetAngleX)
    {
        return new Vector3(targetAngleX, 0, 0);
    }
}