using System.Collections;
using UnityEngine;

public class GridDragMovement : MonoBehaviour
{
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private GridSystem _gridSystem;

    private float _moveDuration;
    private float _cellSize;
    private float _delay;
    private Vector3 _originalPosition;
    private Vector3 _lastTouchWorldPosition;
    private Vector3 _accumulatedWorldDisplacement;
    private Vector2Int _currentGridPosition;
    private WaitForSeconds _waitInitialization;
    private Block  _block;
    private Transform _transform;
    private Coroutine _coroutine;

    private void Awake()
    {
        _delay = 0.1f;
        _moveDuration = 0.15f;
        _waitInitialization = new WaitForSeconds(_delay);
        _block = GetComponent<Block>();

        _transform = transform;
    }

    private IEnumerator Start()
    {
        yield return _waitInitialization;

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
        _originalPosition = _transform.position;
        _currentGridPosition = _gridSystem.WorldToGridPosition(_originalPosition);
        _lastTouchWorldPosition = CalculateTouchWorldPosition(touchPosition);
        _accumulatedWorldDisplacement = Vector3.zero;
        _gridSystem.ClearCell(_currentGridPosition);
    }

    public void ProcessInput(Vector3 touchPosition)
    {
        Vector3 worldTouchPoint = CalculateTouchWorldPosition(touchPosition);
        Vector3 delta = worldTouchPoint - _lastTouchWorldPosition;
        _lastTouchWorldPosition = worldTouchPoint;
        _accumulatedWorldDisplacement += delta;

        if (_accumulatedWorldDisplacement.sqrMagnitude > _cellSize * _cellSize)
            AttemptShift();
    }

    public void EndInteraction()
    {
        if (_gridSystem.WorldToGridPosition(_transform.position) != _currentGridPosition)
            PositionAtCell(_gridSystem.WorldToGridPosition(_transform.position));

        _accumulatedWorldDisplacement = Vector3.zero;
    }

    private void PositionAllBlocks()
    {
        foreach (var block in _blocksContainer.Blocks)
        {
            Vector2Int gridPosition = ClampToGridBounds(_gridSystem.WorldToGridPosition(block.transform.position));
            block.transform.position = _gridSystem.GridToWorldPosition(gridPosition);
            _gridSystem.UpdateCell(gridPosition, block.gameObject);
        }
    }

    private void PositionAtCell(Vector2Int newGridPosition)
    {
        newGridPosition = ClampToGridBounds(newGridPosition);
        if (_gridSystem.IsCellEmpty(newGridPosition) == false) return;

        _gridSystem.ClearCell(_currentGridPosition);
        _gridSystem.UpdateCell(newGridPosition, gameObject);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(SmoothMoveCoroutine(_gridSystem.GridToWorldPosition(newGridPosition)));

        _currentGridPosition = newGridPosition;
    }

    private IEnumerator SmoothMoveCoroutine(Vector3 targetPosition)
    {
        Vector3 startPosition = _transform.position;
        float elapsed = 0f;

        while (elapsed < _moveDuration)
        {
            _transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / _moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _transform.position = targetPosition;
    }

    private void AttemptShift()
    {
        float absDx = Mathf.Abs(_accumulatedWorldDisplacement.x / _cellSize);
        float absDz = Mathf.Abs(_accumulatedWorldDisplacement.z / _cellSize);

        if (HasSufficientMovement(absDx, absDz) == false) return;

        Vector2Int shiftDirection = GetShiftDirection(absDx, absDz);
        Vector2Int newGridPos = CalculateNewGridPosition(shiftDirection);

        if (CanShiftToPosition(newGridPos) == false)
        {
            _accumulatedWorldDisplacement = Vector3.zero;
            return;
        }

        ExecuteShift(newGridPos);
    }

    private bool HasSufficientMovement(float absDx, float absDz)
    {
        return Mathf.Max(absDx, absDz) >= 0.8f;
    }

    private Vector2Int GetShiftDirection(float absDx, float absDz)
    {
        return absDx >= absDz ?
            new Vector2Int(_accumulatedWorldDisplacement.x > 0 ? 1 : -1, 0) :
            new Vector2Int(0, _accumulatedWorldDisplacement.z > 0 ? 1 : -1);
    }

    private Vector2Int CalculateNewGridPosition(Vector2Int shiftDirection)
    {
        return ClampToGridBounds(_currentGridPosition + shiftDirection);
    }

    private bool CanShiftToPosition(Vector2Int newGridPosition)
    {
        return newGridPosition != _currentGridPosition &&
               _gridSystem.IsCellEmpty(newGridPosition);
    }

    private void ExecuteShift(Vector2Int newGridPosition)
    {
        PositionAtCell(newGridPosition);
        _accumulatedWorldDisplacement = Vector3.zero;
    }

    private Vector3 CalculateTouchWorldPosition(Vector3 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        return new Plane(Vector3.up, _originalPosition).Raycast(ray, out float distance) ?
            ray.GetPoint(distance) :
            _originalPosition;
    }

    private Vector2Int ClampToGridBounds(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, _gridSystem.GridSizeX - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, _gridSystem.GridSizeY - 1);

        return gridPosition;
    }
}