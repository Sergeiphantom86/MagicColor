using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Block))]
public class GridMover : MonoBehaviour
{
    private Vector2Int _currentGridPos;
    private Vector2Int? _bufferedDirection;
    private bool _isMoving;

    private Block _block;
    private GridSystem _grid;

    private void Awake()
    {
        _block = GetComponent<Block>();
        _grid = GridSystem.Instance;
        _currentGridPos = _block.GridPosition;
    }

    public void TryMove(Vector2Int direction)
    {
        if (_isMoving)
        {
            _bufferedDirection = direction;
            return;
        }

        ExecuteMove(direction);
    }

    private void ExecuteMove(Vector2Int direction)
    {
        Vector2Int target = _currentGridPos + direction;

        _grid.ClearBlock(_block);

        if (!_grid.CanPlaceBlock(target, _block.SizeInCells))
        {
            _grid.PlaceBlock(_currentGridPos, _block);
            return;
        }

        _isMoving = true;
        _grid.PlaceBlock(target, _block);

        transform.DOMove(
            _grid.GridToWorldPosition(target),
            0.15f
        ).OnComplete(() =>
        {
            _currentGridPos = target;
            _isMoving = false;

            if (_bufferedDirection.HasValue)
            {
                var dir = _bufferedDirection.Value;
                _bufferedDirection = null;
                ExecuteMove(dir);
            }
        });
    }
}
