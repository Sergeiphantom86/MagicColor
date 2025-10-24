using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GridDragMovement1 : MonoBehaviour
{
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private GridSystem _gridSystem;

    private float _moveDuration = 0.15f;
    private Transform _transform;

    private Vector3 _originalPosition;
    private Vector3 _lastTouchWorldPosition;
    private Vector2Int _currentGridPosition;
    private Vector2Int _previousGridPosition; // Добавляем запоминание предыдущей позиции
    private Tween _currentTween;
    private bool _isDragging = false;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        InitializeGridSystem();
        PositionAllBlocks();
        // Запоминаем начальную позицию как предыдущую
        _previousGridPosition = _gridSystem.WorldToGridPosition(_transform.position);
    }

    public void BeginInteraction(Vector3 touchPosition)
    {
        _isDragging = true;
        _originalPosition = _transform.position;
        _currentGridPosition = _gridSystem.WorldToGridPosition(_originalPosition);
        _previousGridPosition = _currentGridPosition; // Запоминаем предыдущую позицию
        _lastTouchWorldPosition = CalculateTouchWorldPosition(touchPosition, _originalPosition);
        _gridSystem.ClearCell(_currentGridPosition);
    }

    public void ProcessInput(Vector3 touchPosition)
    {
        if (!_isDragging) return;

        Vector3 worldTouchPoint = CalculateTouchWorldPosition(touchPosition, _originalPosition);
        Vector3 delta = worldTouchPoint - _lastTouchWorldPosition;
        _lastTouchWorldPosition = worldTouchPoint;

        // Плавно перемещаем объект вслед за пальцем/мышью
        _transform.position += delta;

        // Проверяем, достаточно ли переместились для смены ячейки
        CheckForGridChange();
    }

    private void CheckForGridChange()
    {
        Vector2Int newGridPosition = _gridSystem.WorldToGridPosition(_transform.position);
        newGridPosition = ClampToGridBounds(newGridPosition);

        // Если позиция в сетке изменилась
        if (newGridPosition != _currentGridPosition)
        {
            // Если новая ячейка свободна - перемещаемся
            if (_gridSystem.IsCellEmpty(newGridPosition))
            {
                _previousGridPosition = _currentGridPosition; // Запоминаем предыдущую позицию
                _gridSystem.ClearCell(_currentGridPosition);
                _gridSystem.UpdateCell(newGridPosition, gameObject);
                _currentGridPosition = newGridPosition;
            }
            // Если ячейка занята - остаемся на месте
            else
            {
                // Можно добавить визуальную индикацию (например, легкое сотрясение)
                ShakeBlock();
            }
        }
    }

    public void EndInteraction()
    {
        _isDragging = false;

        // Определяем финальную позицию в сетке
        Vector2Int finalGridPosition = _gridSystem.WorldToGridPosition(_transform.position);
        finalGridPosition = ClampToGridBounds(finalGridPosition);

        // Если целевая ячейка занята или за пределами сетки - возвращаем на предыдущую позицию
        if (!_gridSystem.IsCellEmpty(finalGridPosition) || !IsWithinGridBounds(finalGridPosition))
        {
            ReturnToPreviousPosition();
        }
        else
        {
            // Иначе перемещаем в новую позицию
            PositionAtCell(finalGridPosition);
        }
    }

    // Возврат на предыдущую позицию
    private void ReturnToPreviousPosition()
    {
        // Восстанавливаем занятость предыдущей ячейки
        _gridSystem.UpdateCell(_previousGridPosition, gameObject);

        // Плавно возвращаем блок на предыдущую позицию
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        _currentTween = _transform.DOMove(
            _gridSystem.GridToWorldPosition(_previousGridPosition),
            _moveDuration
        ).SetEase(Ease.OutQuad);

        _currentGridPosition = _previousGridPosition;
    }

    // Визуальная индикация невозможности перемещения
    private void ShakeBlock()
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        // Легкое сотрясение блока
        _currentTween = _transform.DOShakePosition(0.2f, 0.1f).SetEase(Ease.OutQuad);
    }

    private void PositionAtCell(Vector2Int gridPosition)
    {
        gridPosition = ClampToGridBounds(gridPosition);

        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        _currentTween = _transform.DOMove(
            _gridSystem.GridToWorldPosition(gridPosition),
            _moveDuration
        ).SetEase(Ease.OutQuad);

        _currentGridPosition = gridPosition;
        _gridSystem.UpdateCell(gridPosition, gameObject);
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

    private void InitializeGridSystem()
    {
        _gridSystem = GridSystem.Instance;
        if (_gridSystem == null)
            Debug.LogError("GridSystem not found! Add GridSystem component to an object.");
    }

    private Vector3 CalculateTouchWorldPosition(Vector3 touchPosition, Vector3 originalPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        return new Plane(Vector3.up, originalPosition).Raycast(ray, out float distance) ?
            ray.GetPoint(distance) :
            originalPosition;
    }

    // Проверка, находится ли позиция в пределах сетки
    private bool IsWithinGridBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < _gridSystem.GridSizeX &&
               gridPosition.y >= 0 && gridPosition.y < _gridSystem.GridSizeY;
    }

    // Ограничение позиции в пределах сетки
    private Vector2Int ClampToGridBounds(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, _gridSystem.GridSizeX - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, _gridSystem.GridSizeY - 1);
        return gridPosition;
    }
}