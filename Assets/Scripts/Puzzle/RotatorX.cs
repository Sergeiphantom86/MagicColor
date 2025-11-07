using DG.Tweening;
using UnityEngine;

public class RotatorX : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _duration;
    [SerializeField] private float _targetAngleX;
    [SerializeField] private RotateMode _rotateMode;
    [SerializeField] private Ease _easeType;

    [Header("Loop Settings")]
    [SerializeField] private int _loops;
    [SerializeField] private LoopType _loopType;

    private Tween _rotationTween;

    private float _returnAngle;

    private void Awake()
    {
        _returnAngle = 45;
    }

    public void StartRotation()
    {
        SetTargetAngle(_targetAngleX);
    }

    public void Return()
    {
        SetTargetAngle(_returnAngle);
    }

    private void SetTargetAngle(float targetAngleX)
    {
        _rotationTween?.Kill();

        _rotationTween = transform.DORotate(new Vector3(targetAngleX, 0, 0), _duration, _rotateMode)
            .SetEase(_easeType)
            .SetLoops(_loops, _loopType);
    }

    private void OnDestroy()
    {
        _rotationTween?.Kill();
    }
}