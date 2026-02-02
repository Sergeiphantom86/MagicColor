using UnityEngine;

public class SwipeDetector
{
    private const float SwipeThreshold = 30f;

    private Vector2 _startPos;
    private bool _active;

    public bool TryGetSwipe(Vector2 currentPos, out Vector2Int direction)
    {
        direction = Vector2Int.zero;

        if (_active == false)
        {
            _startPos = currentPos;
            _active = true;
            return false;
        }

        Vector2 delta = currentPos - _startPos;

        if (delta.magnitude < SwipeThreshold)
            return false;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            direction = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            direction = delta.y > 0 ? Vector2Int.up : Vector2Int.down;

        _startPos = currentPos;
        return true;
    }

    public void Reset() => _active = false;
}