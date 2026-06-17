using DG.Tweening;
using UnityEngine;

public class DropExplosionAnimator : MonoBehaviour, IDropAnimation
{
    private float _stagger;
    private float _duration;
    private float _minRadius;
    private float _maxRadius;
    private float _minJumpPower;
    private float _maxJumpPower;

    private void Awake()
    {
        _stagger = 0.1f;
        _duration = 0.5f;
        _minRadius = 0.2f;
        _maxRadius = 0.5f;
        _minJumpPower = 1;
        _maxJumpPower = 2;
    }

    public void Play(Vector3 origin)
    {
        AnimateJump(GetTargetPosition(origin), GetRandomJumpPower(), GetRandomDelay());
    }

    private void AnimateJump(Vector3 target, float jumpPower, float delay)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(delay);

        sequence.Append(
            transform.DOJump(target, jumpPower, 1, _duration)
                .SetEase(Ease.OutBounce));
    }

    private Vector3 GetTargetPosition(Vector3 origin)
    {
        return CalculateTarget(origin, GetRandomDirection(), GetRandomRadius());
    }

    private Vector3 GetRandomDirection()
    {
        Vector2 circle = Random.insideUnitCircle.normalized;
        return new Vector3(circle.x, 0f, circle.y);
    }

    private float GetRandomRadius()
    {
        return Random.Range(_minRadius, _maxRadius);
    }

    private float GetRandomJumpPower()
    {
        return Random.Range(_minJumpPower, _maxJumpPower);
    }

    private Vector3 CalculateTarget(Vector3 origin, Vector3 direction, float radius)
    {
        Vector3 target = origin + direction * radius;

        target.y = origin.y;

        return target;
    }

    private float GetRandomDelay()
    {
        return Random.Range(0f, _stagger);
    }
}