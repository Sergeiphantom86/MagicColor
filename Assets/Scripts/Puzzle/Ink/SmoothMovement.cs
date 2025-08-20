using UnityEngine;

public class SmoothMoveToTarget : MonoBehaviour
{
    [SerializeField] private Pen _target;
    [SerializeField] private Transform _waypoint;

    private SmoothAppearance _smoothAppearance;
    private bool _isMoving;
    private float _minDistance;
    private float _delayTimer;
    private float _movementSpeed;
    private bool _reachedWaypoint;
    private float _waypointXOffset;
    private float _randomXOffset;
    private Vector3 _modifiedWaypointPosition;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        _waypointXOffset = 0.5f;
        _delayTimer = 1;
        _movementSpeed = 20;
        _minDistance = 0.1f;
        _isMoving = false;
        _smoothAppearance = GetComponent<SmoothAppearance>();
        _reachedWaypoint = false;
        _randomXOffset = 0f;
        _modifiedWaypointPosition = Vector3.zero;
    }

    private void Update()
    {
        if (IsMoving == false) return;

        ProcessMovementDelay();

        if (_isMoving == false || _target == null) return;

        UpdatePosition();
        VerifyDestinationReached();
    }

    public void BeginMovement()
    {
        IsMoving = true;
        _randomXOffset = Random.Range(-_waypointXOffset, _waypointXOffset);

        if (_waypoint != null)
        {
            _modifiedWaypointPosition = _waypoint.position;
            _modifiedWaypointPosition.x += _randomXOffset;
        }
    }

    private void UpdatePosition()
    {
        transform.position = Vector3.MoveTowards(transform.position,DetermineDestination(),_movementSpeed * Time.deltaTime);
    }

    private void VerifyDestinationReached()
    {
        if (CheckWaypointArrival()) return;
    }

    private bool CheckWaypointArrival()
    {
        if (_reachedWaypoint || _waypoint == null)
            return false;

        if (CalculateDistance(transform.position, _modifiedWaypointPosition) <= _minDistance)
        {
            _reachedWaypoint = true;
            _smoothAppearance.Hide();
            return true;
        }

        return false;
    }

    private float CalculateDistance(Vector3 position, Vector3 target)
    {
        return Vector3.Distance(position, target);
    }

    private Vector3 DetermineDestination()
    {
        if (_reachedWaypoint || _waypoint == null)
            return _target.transform.position;

        return _modifiedWaypointPosition;
    }

    private void ProcessMovementDelay()
    {
        if (_isMoving) return;

        _delayTimer -= Time.deltaTime;

        if (_delayTimer <= 0)
        {
            _isMoving = true;
        }
    }
}