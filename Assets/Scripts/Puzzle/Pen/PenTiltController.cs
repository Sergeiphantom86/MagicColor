using UnityEngine;
using DG.Tweening;

public class PenTiltController : MonoBehaviour
{
    [SerializeField] private float _maxTiltAngle = 30f;
    [SerializeField] private float _maxDistance = 5f;
    [SerializeField] private float _positionThreshold = 0.01f;
    [SerializeField] private float _tiltDuration = 1f;

    public float MaxTiltAngle => _maxTiltAngle;
    public float MaxDistance => _maxDistance;
    public float PositionThreshold => _positionThreshold;

    private float _currentAngle;
    private Tween _tiltTween;

    public void Initialize(float startAngle)
    {
        _currentAngle = startAngle;
    }

    public void ApplyTilt(float targetAngle)
    {
        if (Mathf.Abs(targetAngle - _currentAngle) > _positionThreshold)
        {
            StartTween(targetAngle);
        }
    }

    public void Stop()
    {
        _tiltTween?.Kill();
    }

    private void StartTween(float targetAngle)
    {
        _tiltTween?.Kill();

        _tiltTween = DOTween.To(() => _currentAngle,
                angle => {
                    _currentAngle = angle;
                    transform.localEulerAngles = new Vector3(0, 0, angle);
                },
                targetAngle,_tiltDuration
            )
            .SetEase(Ease.OutQuad);
    }
}