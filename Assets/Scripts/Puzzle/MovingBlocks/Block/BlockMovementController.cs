using System;
using UnityEngine;

public class BlockMovementController
{
    private readonly PathMover _pathMover;
    private readonly GridDragMovement _gridDragMovement;
    private readonly ITouchDragInput _touchDragInput;

    public event Action Moved;

    public BlockMovementController(PathMover pathMover, GridDragMovement gridDragMovement, ITouchDragInput touchDragInput)
    {
        _pathMover = pathMover;
        _gridDragMovement = gridDragMovement;
        _touchDragInput = touchDragInput;

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (_gridDragMovement != null)
            _gridDragMovement.Moved += OnMoved;
    }

    private void UnsubscribeEvents()
    {
        if (_gridDragMovement != null)
            _gridDragMovement.Moved -= OnMoved;
    }

    private void OnMoved()
    {
        Moved?.Invoke();
    }

    public void ThrowOff()
    {
        _touchDragInput?.ThrowOff();
    }

    public void Move(Vector3 waypoint, Vector3 endPoint, Action onComplete = null)
    {
        if (_pathMover == null) return;

        _pathMover.Move(waypoint, endPoint, onComplete);
    }

    public void Dispose()
    {
        UnsubscribeEvents();
    }
}
