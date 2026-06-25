using System.Collections;
using PuzzleEditor.PenEditor;
using UnityEngine;

namespace PuzzleEditor.InkEditor
{
    [RequireComponent(typeof(SmoothAppearance), typeof(UIMaterialOrder))]
    public class SmoothMoveToTarget : MonoBehaviour
    {
        [SerializeField]
        private PenVisualer _target;

        [SerializeField]
        private Transform _waypoint;

        private bool _isMoving;
        private float _minDistance;
        private float _delayTimer;
        private float _movementSpeed;
        private bool _reachedWaypoint;
        private float _waypointXOffset;
        private Vector3 _modifiedWaypointPosition;
        private UIMaterialOrder _materialOrder;
        private SmoothAppearance _smoothAppearance;
        private Drop _drop;

        public bool IsMoving { get; private set; }

        private void Awake()
        {
            _delayTimer = 1;
            _movementSpeed = 20;
            _minDistance = 0.1f;
            _waypointXOffset = 0.5f;

            _isMoving = false;
            _reachedWaypoint = false;
            _drop = GetComponent<Drop>();
            _smoothAppearance = GetComponent<SmoothAppearance>();
            _materialOrder = GetComponent<UIMaterialOrder>();
        }

        private void Update()
        {
            if (IsMoving == false)
                return;

            ProcessMovementDelay();

            StartCoroutine(Wait());

            if (_isMoving == false || _target == null)
                return;

            UpdatePosition();

            if (CheckWaypointArrival())
                return;
        }

        public void BeginMovement()
        {
            IsMoving = true;

            transform.SetParent(null);

            if (_waypoint != null)
            {
                _modifiedWaypointPosition = _waypoint.position;
                _modifiedWaypointPosition.x += GetRandomXOffset();
            }

            _drop.PlaySoundMoving();
        }

        private float GetRandomXOffset()
        {
            return Random.Range(-_waypointXOffset, _waypointXOffset);
        }

        private void UpdatePosition()
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                DetermineDestination(),
                _movementSpeed * Time.deltaTime
            );
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
            if (_isMoving)
                return;

            _delayTimer -= Time.deltaTime;

            if (_delayTimer <= 0)
            {
                _isMoving = true;
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(1f);

            _materialOrder.SetOrder();
        }
    }
}