using DG.Tweening;
using UnityEngine;

public class GridDragMovement : MonoBehaviour
{
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private float _shiftThreshold = 0.5f;

    private float _moveDuration = 0.15f;
    private Transform _transform;

    private Vector3 _lastTouchWorldPosition;
    private Vector3 _accumulatedWorldDisplacement;
    private Vector2Int _currentGridPosition;
    private Tween _currentTween;
    private bool _hasMoved = false;

    // Инициализация компонента
    private void Awake()
    {
        _transform = transform;
    }

    // Запуск после инициализации, подготовка сетки
    private void Start()
    {
        InitializeGridSystem();
        PositionAllBlocks();
    }

    // Начало взаимодействия: фиксация начальной позиции
    public void BeginInteraction(Vector3 touchPosition, Vector3 originalPosition)
    {
        _currentGridPosition = _gridSystem.WorldToGridPosition(originalPosition);
        _lastTouchWorldPosition = CalculateTouchWorldPosition(touchPosition, originalPosition);
        _accumulatedWorldDisplacement = Vector3.zero;
        _hasMoved = false;
    }

    // Обработка перемещения: вычисление смещения
    public void ProcessInput(Vector3 touchPosition, Vector3 originalPosition)
    {
        Vector3 worldTouchPoint = CalculateTouchWorldPosition(touchPosition, originalPosition);
        Vector3 delta = worldTouchPoint - _lastTouchWorldPosition;
        _lastTouchWorldPosition = worldTouchPoint;
        _accumulatedWorldDisplacement += delta;

        float cellSize = _gridSystem.CellSize;

        if (_accumulatedWorldDisplacement.sqrMagnitude > cellSize)
        {
            _hasMoved = true;
            AttemptShift(_accumulatedWorldDisplacement);
        }
    }

    // Завершение взаимодействия: финальное позиционирование
    public void EndInteraction(Vector3 position)
    {
        if (_hasMoved == false)
        {
            PositionAtCell(_currentGridPosition);
        }
        else if (_gridSystem.WorldToGridPosition(position) != _currentGridPosition)
        {
            PositionAtCell(_gridSystem.WorldToGridPosition(position));
        }

        _accumulatedWorldDisplacement = Vector3.zero;
    }

    // Попытка сдвига блока при достаточном перемещении
    private void AttemptShift(Vector3 accumulatedWorldDisplacement)
    {
        float absDx = Mathf.Abs(accumulatedWorldDisplacement.x);
        float absDz = Mathf.Abs(accumulatedWorldDisplacement.z);

        Vector2Int shiftDirection = GetShiftDirection(absDx, absDz);
        Vector2Int newGridPos = CalculateNewGridPosition(shiftDirection);

        if (CanShiftToPosition(newGridPos) == false) return;

        _gridSystem.ClearCell(_currentGridPosition);
        ExecuteShift(newGridPos);
    }

    // Определение направления сдвига
    private Vector2Int GetShiftDirection(float absDx, float absDz)
    {
        return absDx >= absDz ?
            new Vector2Int(_accumulatedWorldDisplacement.x > 0 ? 1 : -1, 0) :
            new Vector2Int(0, _accumulatedWorldDisplacement.z > 0 ? 1 : -1);
    }

    // Вычисление новой позиции в сетке
    private Vector2Int CalculateNewGridPosition(Vector2Int shiftDirection) =>
        ClampToGridBounds(_currentGridPosition + shiftDirection);

    // Проверка возможности перемещения в ячейку
    private bool CanShiftToPosition(Vector2Int newGridPosition) =>
        newGridPosition != _currentGridPosition && _gridSystem.IsCellEmpty(newGridPosition);

    // Выполнение сдвига и обновление сетки
    private void ExecuteShift(Vector2Int newGridPosition)
    {
        PositionAtCell(newGridPosition);
        _accumulatedWorldDisplacement = Vector3.zero;
    }

    // Позиционирование всех блоков в сетке
    private void PositionAllBlocks()
    {
        foreach (var block in _blocksContainer.Blocks)
        {
            Vector2Int gridPosition = ClampToGridBounds(_gridSystem.WorldToGridPosition(block.transform.position));
            block.transform.position = _gridSystem.GridToWorldPosition(gridPosition);
            _gridSystem.UpdateCell(gridPosition, block.gameObject);
        }
    }

    // Инициализация системы сетки
    private void InitializeGridSystem()
    {
        _gridSystem = GridSystem.Instance;
        if (_gridSystem == null)
            Debug.LogError("GridSystem not found! Add GridSystem component to an object.");
    }

    // Плавное перемещение в конкретную ячейку
    private void PositionAtCell(Vector2Int newGridPosition)
    {
        newGridPosition = ClampToGridBounds(newGridPosition);
        if (_gridSystem.IsCellEmpty(newGridPosition) == false) return;

        _gridSystem.ClearCell(_currentGridPosition);
        _gridSystem.UpdateCell(newGridPosition, gameObject);

        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        _currentTween = _transform.DOMove(
            _gridSystem.GridToWorldPosition(newGridPosition),
            _moveDuration
        ).SetEase(Ease.OutQuad);

        _currentGridPosition = newGridPosition;
    }

    // Преобразование экранных координат в мировые
    private Vector3 CalculateTouchWorldPosition(Vector3 touchPosition, Vector3 originalPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        return new Plane(Vector3.up, originalPosition).Raycast(ray, out float distance) ?
            ray.GetPoint(distance) :
            originalPosition;
    }

    // Ограничение позиции в пределах сетки
    private Vector2Int ClampToGridBounds(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, _gridSystem.GridSizeX - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, _gridSystem.GridSizeY - 1);
        return gridPosition;
    }
}