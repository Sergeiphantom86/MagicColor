using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _collisionCheckRadius = 0.5f;
    [SerializeField] private float _stoppingDistance = 0.01f;

    private CollisionHandler _collisionHandler;
    private Vector3 _currentTarget;
    private bool _isDragging;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _collisionHandler = GetComponent<CollisionHandler>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void StartDragging()
    {
        _isDragging = true;
        _rigidbody.isKinematic = false;
    }

    public void StopDragging()
    {
        _isDragging = false;
        _rigidbody.isKinematic = true;
    }

    public void HandleMovement(Vector3 newPosition)
    {
        _currentTarget = newPosition;

        if (_isDragging == false) return;
        if (HasReachedTarget() || IsPathBlocked()) return;

        MoveToTarget();
    }

    public bool ConfirmMovement()
    {
        return HasReachedTarget();
    }

    private bool HasReachedTarget()
    {
        return Vector3.SqrMagnitude(_currentTarget - transform.position)
               < _stoppingDistance * _stoppingDistance;
    }

    private bool IsPathBlocked()
    {
        if (_collisionHandler.IsCollidingWithObstacle) return true;

        Vector3 toTarget = _currentTarget - transform.position;
        float distance = toTarget.magnitude;
        Vector3 direction = toTarget.normalized;

        return _collisionHandler.CheckPathObstructed(transform.position, direction,distance,_collisionCheckRadius);
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position,_currentTarget,_speed * Time.deltaTime);
    }
}