using UnityEngine;
using DG.Tweening;

public class PenTiltController : MonoBehaviour
{
    private float _maxTiltAngle;
    private float _tiltDuration;
    private float _currentAngle;
    private Tween _tiltTween;

    private void Awake()
    {
        _maxTiltAngle = 30;
        _tiltDuration = 0.2f;

        _currentAngle = transform.localEulerAngles.z;
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
    }

    private void Update()
    {
        ApplyTiltBasedOnDistance(transform.position.x);
    }

    public void ApplyTiltBasedOnDistance(float distanceFromCenter)
    {
        StartTween(-GetTargetAngele(distanceFromCenter));
    }

    private float GetTargetAngele(float distanceFromCenter)
    {
        return distanceFromCenter * _maxTiltAngle;
    }

    private void StartTween(float targetAngle)
    {
        _tiltTween?.Kill();

        _tiltTween = DOTween.To(() => 
        _currentAngle,angle =>
                {
                    _currentAngle = angle;
                    transform.localEulerAngles = new Vector3(0, 0, angle);
                },
                targetAngle,_tiltDuration);
    }

    private void OnDestroy()
    {
        _tiltTween?.Kill();
    }
}