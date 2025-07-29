using System.Collections;
using UnityEngine;

public class GridDragMovement : MonoBehaviour
{
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private GridSystem _gridSystem;

    private float _cellSize;
    private bool _isDragging;
    private Vector3 _originalPosition;
    private Vector3 _lastTouchWorldPosition;
    private Vector3 _accumulatedWorldDisplacement;
    private Vector2Int _currentGridPosition;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        _gridSystem = GridSystem.Instance;

        if (_gridSystem == null)
        {
            Debug.LogError("GridSystem not found! Add GridSystem component to an object.");
            yield return null;
        }

        _cellSize = _gridSystem.CellSize;
        PositionAllBlocks();
    }

    public void BeginInteraction(Vector3 touchPosition)
    {
        if (_gridSystem == null) return;

        _isDragging = true;
        _originalPosition = transform.position;
        _currentGridPosition = _gridSystem.WorldToGridPosition(_originalPosition);
        _lastTouchWorldPosition = CalculateTouchWorldPosition(touchPosition);
        _accumulatedWorldDisplacement = Vector3.zero;

        _gridSystem.ClearCell(_currentGridPosition);
    }

    public void ProcessInput(Vector3 touchPosition)
    {
        if (_isDragging == false || _gridSystem == null) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector3 worldTouchPoint = CalculateTouchWorldPosition(touchPosition);
            Vector3 delta = worldTouchPoint - _lastTouchWorldPosition;
            _lastTouchWorldPosition = worldTouchPoint;
            _accumulatedWorldDisplacement += delta;

            if (_accumulatedWorldDisplacement.sqrMagnitude > _cellSize * _cellSize)
            {
                AttemptShift();
            }
        }
    }

    public void EndInteraction()
    {
        if (_isDragging == false || _gridSystem == null) return;

        _isDragging = false;

        Vector2Int actualGridPos = _gridSystem.WorldToGridPosition(transform.position);

        if (actualGridPos != _currentGridPosition)
        {
            PositionAtCell(actualGridPos);
        }
        _accumulatedWorldDisplacement = Vector3.zero;
    }

    private void PositionAllBlocks()
    {
        foreach (var block in _blocksContainer.Blocks)
        {
            Vector2Int gridPosition = _gridSystem.WorldToGridPosition(block.transform.position);
            gridPosition = ClampToGridBounds(gridPosition);
            PlaceObjectAtCell(block.transform, gridPosition);
        }
    }

    private void PlaceObjectAtCell(Transform objectTransform, Vector2Int gridPosition)
    {
        Vector3 worldPosition = _gridSystem.GridToWorldPosition(gridPosition);
        objectTransform.position = worldPosition;
        _gridSystem.UpdateCell(gridPosition, objectTransform.gameObject);
    }

    private void PositionAtCell(Vector2Int newGridPosition)
    {
        _gridSystem.ClearCell(_currentGridPosition);

        newGridPosition = ClampToGridBounds(newGridPosition);
        _currentGridPosition = newGridPosition;

        Vector3 worldPosition = _gridSystem.GridToWorldPosition(newGridPosition);
        transform.position = worldPosition;
        _gridSystem.UpdateCell(newGridPosition, gameObject);
    }

    private void AttemptShift()
    {
        Vector2 relativeMovement = new Vector2(
            _accumulatedWorldDisplacement.x / _cellSize,
            _accumulatedWorldDisplacement.z / _cellSize
        );

        float absDx = Mathf.Abs(relativeMovement.x);
        float absDz = Mathf.Abs(relativeMovement.y);
        float primaryAxis = Mathf.Max(absDx, absDz);

        if (primaryAxis < 0.8f) return;

        Vector2Int newGridPos = _currentGridPosition + DetermineShiftDirection(absDx, absDz);
        newGridPos = ClampToGridBounds(newGridPos);

        if (newGridPos == _currentGridPosition)
        {
            _accumulatedWorldDisplacement = Vector3.zero;
            return;
        }

        if (_gridSystem.IsCellEmpty(newGridPos) == false) return;

        PositionAtCell(newGridPos);
        _accumulatedWorldDisplacement = Vector3.zero;
    }

    private Vector2Int DetermineShiftDirection(float absDx, float absDz)
    {
        if (absDx >= absDz)
            return new Vector2Int(_accumulatedWorldDisplacement.x > 0 ? 1 : -1, 0);

        return new Vector2Int(0, _accumulatedWorldDisplacement.z > 0 ? 1 : -1);
    }

    private Vector3 CalculateTouchWorldPosition(Vector3 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        return CreateInteractionPlane().Raycast(ray, out float distance)
            ? ray.GetPoint(distance)
            : _originalPosition;
    }

    private Plane CreateInteractionPlane() =>
        new(Vector3.up, _originalPosition);

    private Vector2Int ClampToGridBounds(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, GridSystem.Instance.GridSizeX - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, GridSystem.Instance.GridSizeY - 1);
        return gridPosition;
    }
}