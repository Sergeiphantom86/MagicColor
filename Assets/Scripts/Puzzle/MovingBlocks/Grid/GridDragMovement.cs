using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Block), typeof(ITouchDragInput))]
public class GridDragMovement : MonoBehaviour
{
    [SerializeField] private float _moveDuration;

    private Camera _camera;
    private Transform _transform;
    private GridSystem _grid;
    private Block _block;
    private Vector3 _lastWorldTouch;
    private Vector3 _accumulatedDelta;
    private Vector2Int _currentCenterCell;
    private ITouchDragInput _touchDragInput;
    private int _cellsSinceLastSound = 0;

    private Tween _moveTween;

    public event Action Moved;

    private void Awake()
    {
        _camera = Camera.main;
        _transform = transform;
        _grid = GridSystem.Instance;
        _block = GetComponent<Block>();
        _touchDragInput = GetComponent<ITouchDragInput>();
    }

    private void OnEnable()
    {
        _touchDragInput.OnTouchClick += BeginInteraction;
        _touchDragInput.OnTouchDrag += ProcessInput;
    }

    private void OnDisable()
    {
        _touchDragInput.OnTouchClick -= BeginInteraction;
        _touchDragInput.OnTouchDrag -= ProcessInput;
    }

    public void BeginInteraction(Vector2 screenTouchPos)
    {
        _currentCenterCell = new Vector2Int(
            _block.GridPosition.x + (_block.SizeInCells.x - 1) / 2,
            _block.GridPosition.y + (_block.SizeInCells.y - 1) / 2);

        _lastWorldTouch = ScreenToWorld(screenTouchPos);
        _accumulatedDelta = Vector3.zero;
    }

    public void ProcessInput(Vector2 screenTouchPos)
    {
        Vector3 worldTouch = ScreenToWorld(screenTouchPos);
        Vector3 delta = worldTouch - _lastWorldTouch;

        _lastWorldTouch = worldTouch;
        _accumulatedDelta += delta;

        if (_accumulatedDelta.magnitude >= _grid.CellSize / 2f)
        {
            TryShift();

            _cellsSinceLastSound++;

            if (_cellsSinceLastSound >= 5)
            {
                Moved?.Invoke();

                _cellsSinceLastSound = 0;
            }
        }
    }

    private void TryShift()
    {
        Vector2Int dir = GetShiftDirection(_accumulatedDelta);
        Vector2Int targetCenter = _currentCenterCell + dir;

        Vector2Int targetOrigin =
            _grid.GetOriginFromCenter(targetCenter, _block.SizeInCells);

        _grid.ClearCell(_block);

        if (_grid.CanPlaceBlock(targetOrigin, _block.SizeInCells) == false)
        {
            Vector2Int currentOrigin =
                _grid.GetOriginFromCenter(_currentCenterCell, _block.SizeInCells);

            _grid.PlaceObject(currentOrigin, _block);
            _accumulatedDelta = Vector3.zero;
            return;
        }

        MoveTo(targetCenter);
        _accumulatedDelta = Vector3.zero;
    }

    private void MoveTo(Vector2Int targetCenter)
    {
        _moveTween?.Kill();

        Vector2Int origin =
            _grid.GetOriginFromCenter(targetCenter, _block.SizeInCells);

        _grid.PlaceObject(origin, _block);

        _moveTween = _transform.DOMove(GetWorldCenterFromOrigin(origin), _moveDuration).SetEase(Ease.OutQuad);

        _currentCenterCell = targetCenter;
    }

    private Vector3 GetWorldCenterFromOrigin(Vector2Int origin)
    {
        Vector3 basePos = _grid.GridToWorldPosition(origin);
        Vector2Int size = _block.SizeInCells;

        Vector3 offset = _grid.GetComponent<Grid>().CellToWorld(
            new Vector3Int(size.x - 1, size.y - 1, 0)) - _grid.GetComponent<Grid>().CellToWorld(Vector3Int.zero);

        return basePos + offset * 0.5f;
    }

    private Vector2Int GetShiftDirection(Vector3 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
            return delta.x > 0 ? Vector2Int.right : Vector2Int.left;

        return delta.z > 0 ? Vector2Int.up : Vector2Int.down;
    }

    private Vector3 ScreenToWorld(Vector3 screenPos)
    {
        Ray ray = _camera.ScreenPointToRay(screenPos);
        Plane plane = new(Vector3.up, _transform.position);

        return plane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : _transform.position;
    }
}