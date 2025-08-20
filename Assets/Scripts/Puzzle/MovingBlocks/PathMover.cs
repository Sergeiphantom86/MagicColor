using UnityEngine;
using DG.Tweening;
using System;

public class PathMover : MonoBehaviour
{
    private Ease moveEase;
    private float durationToWaypoint;
    private float durationToEnd;
    private Sequence _pathSequence;

    private void Awake()
    {
        moveEase = Ease.Linear;
        durationToWaypoint = 0.1f;
        durationToEnd = 0.2f;
    }

    public void Move(Transform waypoint, Transform endPoint, Action onComplete = null)
    {
        _pathSequence = DOTween.Sequence();

        AddMovePoint(_pathSequence, waypoint.position, durationToWaypoint);

        AddMovePoint(_pathSequence, endPoint.position, durationToEnd);

        _pathSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private void AddMovePoint(Sequence sequence, Vector3 targetPosition, float duration)
    {
        sequence.Append(transform.DOMove(targetPosition, duration).SetEase(moveEase));
    }

    private void OnDestroy()
    {
        if (_pathSequence != null && _pathSequence.IsActive())
        {
            _pathSequence.Kill();
        }
    }
}