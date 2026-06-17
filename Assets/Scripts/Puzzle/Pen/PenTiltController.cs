using UnityEngine;

public class PenTiltController : MonoBehaviour
{
    [Header("Tilt Settings")]
    [SerializeField] private float _maxTiltAngle;
    [SerializeField] private float _tiltDuration;

    private float _currentAngle;

    private void Awake()
    {
        _currentAngle = transform.localEulerAngles.z;
        transform.localEulerAngles = new Vector3(0, 0, _currentAngle);
    }

    private void Update()
    {
        float targetAngle = GetTargetAngle(transform.position.x);

        if (Mathf.Approximately(targetAngle, _currentAngle) == false)
        {
            _currentAngle = Mathf.Lerp(_currentAngle, targetAngle, Time.deltaTime / _tiltDuration);

            transform.localEulerAngles = new Vector3(0, 0, _currentAngle);
        }
    }

    private float GetTargetAngle(float distanceFromCenter)
    {
        float angle = distanceFromCenter * _maxTiltAngle;
        angle = Mathf.Clamp(angle, -_maxTiltAngle, _maxTiltAngle);
        return -angle;
    }
}