using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private const float StopThreshold = 0.1f;
    private const float AngeleSectorSize = 45f;
    private const float AngeleOffset = 22.5f;
    private const string Speed = nameof(Speed);
    private const string Direction = nameof(Direction);

    private Animator _animator;
    private int _angleFullCircle;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _angleFullCircle = 360;
    }

    public void UpdateAnimation(Vector3 velocity, float speed)
    {
        _animator.SetFloat(Speed, speed);

        if (speed < StopThreshold)
        {
            _animator.SetFloat(Direction, 0);
            return;
        }

        CalculateMovementDirection(velocity.normalized);
    }

    private void CalculateMovementDirection(Vector3 normalizedVelocity)
    {
        float normalizedAngle = NormalizeAngle(CalculateRelativeAngle(normalizedVelocity));

        _animator.SetFloat(Direction, ConvertAngleToDirectionIndex(normalizedAngle));
    }

    private float CalculateRelativeAngle(Vector3 normalizedVelocity)
    {
        return Vector3.SignedAngle(transform.forward, normalizedVelocity, Vector3.up);
    }

    private float NormalizeAngle(float angle)
    {
        return angle < 0 ? angle + _angleFullCircle : angle;
    }

    private int ConvertAngleToDirectionIndex(float angle)
    {
        return Mathf.FloorToInt((angle + AngeleOffset) % _angleFullCircle / AngeleSectorSize);
    }
}