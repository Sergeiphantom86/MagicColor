using DG.Tweening;
using System;
using UnityEngine;

public class HandMover : MonoBehaviour
{
    private Vector3 _startScale;
    private Vector3 _targetScale;
    private Sequence _sequence;
    private int _distanceZ;
    private int _distanceX;
    private float _duration;
    private float _overshoot;
    private int _scaleMultiplier;

    public event Action OnDiscontinued;

    private void Awake()
    {
        _distanceZ = 2;
        _distanceX = 3;
        _overshoot = 5;
        _duration = 0.7f;
        _scaleMultiplier = 20;

        _targetScale = Vector3.one * _scaleMultiplier;
        _startScale = transform.localScale;
    }

    private void OnDisable()
    {
        Stop();
    }

    public void EnableScaleAnimation()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(transform
            .DOScale(_targetScale, _duration)
            .SetEase(Ease.OutBack, _overshoot))
            .SetLoops(-1, LoopType.Restart);
    }

    public void EnableMoveAnimationZ()
    {
        GetAnimationSequence(0, _distanceZ)
            .OnComplete(() =>
            OnDiscontinued?.Invoke()); ;
    }

    public void EnableMoveAnimationX()
    {
        GetAnimationSequence(-_distanceX)
            .OnComplete(() => 
            OnDiscontinued?.Invoke());
    }

    public void EnableLoopingAnimationZ()
    {
        GetAnimationSequence(0, _distanceZ)
            .SetLoops(-1, LoopType.Restart);
    }

    public Sequence GetAnimationSequence(float distanceX = 0, float distance = 0)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.AppendInterval(_duration);
        _sequence.Join(transform.DOMove(GetTargetPosition(distanceX, distance), 1f));

        return _sequence;
    }

    public void Stop()
    {
        _sequence?.Kill();
        transform.localScale = Vector3.one * _startScale.x;
    }

    private Vector3 GetTargetPosition(float distanceX = 0, float distance = 0)
    {
        Vector3 position  = transform.position;

        position.x -= distanceX;
        position.z -= distance;

        return position;
    }
}