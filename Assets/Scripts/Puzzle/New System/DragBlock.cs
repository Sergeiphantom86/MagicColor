using UnityEngine;
using System.Collections;

public class GridBlockDragger : MonoBehaviour
{
    [Header("Grid Settings")]
    public GridManagerXZ gridManager; // Ссылка на менеджер сетки
    public bool snapToGrid = true;

    [Header("Block Settings")]
    public int widthInCells = 1;
    public int depthInCells = 1;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float liftHeight = 0.5f;

    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private bool isMovingToTarget = false;
    private float originalY;
    private Vector2Int gridPosition;

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
        targetPosition = transform.position;
        originalY = transform.position.y;

        // Если gridManager не назначен, пытаемся найти его
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManagerXZ>();
        }

        // Инициализируем позицию в сетке
        UpdateBlockSize();
        SnapToGridImmediate();
    }

    void Update()
    {
        HandleInput();

        // Плавное перемещение к целевой позиции
        if (isMovingToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Если достигли цели, останавливаемся
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMovingToTarget = false;

                // Обновляем позицию в сетке после завершения движения
                UpdateGridPosition();
            }
        }
    }

    void HandleInput()
    {
        // Обработка клика мыши
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    StartDragging();
                }
            }
        }

        // Обработка перемещения мыши
        if (isDragging && Input.GetMouseButton(0))
        {
            DragObject();
        }

        // Обработка отпускания мыши
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }

        // Обработка касаний на мобильных устройствах
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        StartDragging();
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                DragObject();
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isDragging)
            {
                StopDragging();
            }
        }
    }

    void StartDragging()
    {
        isDragging = true;
        isMovingToTarget = false;

        // Освобождаем ячейки в сетке
        if (gridManager != null)
        {
            gridManager.ReleaseArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);
        }

        // Поднимаем блок немного над поверхностью
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, originalY + liftHeight, currentPos.z);

        // Рассчитываем смещение между позицией блока и точкой касания
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 touchWorldPosition = hit.point;
            touchWorldPosition.y = transform.position.y;
            offset = transform.position - touchWorldPosition;
        }
    }

    void DragObject()
    {
        // Получаем мировые координаты касания на плоскости XZ
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 touchWorldPosition = hit.point;
            touchWorldPosition.y = transform.position.y; // Сохраняем текущую высоту

            // Вычисляем новую позицию с учетом смещения
            Vector3 newPosition = touchWorldPosition + offset;

            // Привязываем к сетке если нужно
            if (snapToGrid && gridManager != null)
            {
                // Преобразуем мировые координаты в координаты сетки
                Vector2Int gridCoords = gridManager.WorldToGridPosition(newPosition);

                // Проверяем, доступна ли область
                if (gridManager.IsAreaAvailable(gridCoords.x, 0, gridCoords.y, widthInCells, depthInCells))
                {
                    // Преобразуем координаты сетки обратно в мировые
                    newPosition = gridManager.GridToWorldPosition(gridCoords.x, gridCoords.y);
                    newPosition.y = transform.position.y;

                    // Учитываем размер блока
                    newPosition.x += (widthInCells - 1) * gridManager.gridSize / 2f;
                    newPosition.z += (depthInCells - 1) * gridManager.gridSize / 2f;
                }
            }

            // Непосредственно перемещаем блок
            transform.position = newPosition;
        }
    }

    void StopDragging()
    {
        isDragging = false;

        // Фиксируем окончательную позицию с учетом сетки
        Vector3 finalPosition = transform.position;

        if (snapToGrid && gridManager != null)
        {
            // Преобразуем мировые координаты в координаты сетки
            Vector2Int gridCoords = gridManager.WorldToGridPosition(finalPosition);

            // Проверяем, доступна ли область
            if (!gridManager.IsAreaAvailable(gridCoords.x, 0, gridCoords.y, widthInCells, depthInCells))
            {
                // Если область занята, возвращаемся на предыдущую позицию
                finalPosition = gridManager.GridToWorldPosition(gridPosition.x, gridPosition.y);
                finalPosition.x += (widthInCells - 1) * gridManager.gridSize / 2f;
                finalPosition.z += (depthInCells - 1) * gridManager.gridSize / 2f;
            }
            else
            {
                // Обновляем позицию в сетке
                gridPosition = gridCoords;

                // Занимаем область в сетке
                gridManager.OccupyArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);

                // Точное позиционирование по сетке
                finalPosition = gridManager.GridToWorldPosition(gridPosition.x, gridPosition.y);
                finalPosition.x += (widthInCells - 1) * gridManager.gridSize / 2f;
                finalPosition.z += (depthInCells - 1) * gridManager.gridSize / 2f;
            }
        }

        // Опускаем блок на исходную высоту
        finalPosition.y = originalY;

        // Устанавливаем целевую позицию для плавного перемещения
        targetPosition = finalPosition;
        isMovingToTarget = true;
    }

    // Немедленная привязка к сетке
    void SnapToGridImmediate()
    {
        if (gridManager != null)
        {
            // Преобразуем мировые координаты в координаты сетки
            gridPosition = gridManager.WorldToGridPosition(transform.position);

            // Проверяем, доступна ли область
            if (!gridManager.IsAreaAvailable(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells))
            {
                // Ищем ближайшую доступную позицию
                FindAvailablePosition();
            }

            // Занимаем область в сетке
            gridManager.OccupyArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);

            // Точное позиционирование по сетке
            Vector3 newPosition = gridManager.GridToWorldPosition(gridPosition.x, gridPosition.y);
            newPosition.x += (widthInCells - 1) * gridManager.gridSize / 2f;
            newPosition.z += (depthInCells - 1) * gridManager.gridSize / 2f;
            newPosition.y = originalY;

            transform.position = newPosition;
            targetPosition = newPosition;
        }
    }

    // Поиск доступной позиции
    void FindAvailablePosition()
    {
        if (gridManager == null) return;

        // Простой алгоритм поиска ближайшей доступной позиции
        for (int radius = 1; radius < 10; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    if (Mathf.Abs(x) == radius || Mathf.Abs(z) == radius)
                    {
                        int checkX = gridPosition.x + x;
                        int checkZ = gridPosition.y + z;

                        if (gridManager.IsAreaAvailable(checkX, 0, checkZ, widthInCells, depthInCells))
                        {
                            gridPosition = new Vector2Int(checkX, checkZ);
                            return;
                        }
                    }
                }
            }
        }
    }

    // Обновление позиции в сетке
    void UpdateGridPosition()
    {
        if (gridManager != null)
        {
            gridPosition = gridManager.WorldToGridPosition(transform.position);
            gridManager.OccupyArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);
        }
    }

    // Метод для изменения размера блока
    public void SetSize(int width, int depth)
    {
        // Освобождаем старую область в сетке
        if (gridManager != null)
        {
            gridManager.ReleaseArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);
        }

        widthInCells = width;
        depthInCells = depth;
        UpdateBlockSize();

        // Занимаем новую область в сетке
        if (gridManager != null)
        {
            // Проверяем, доступна ли новая область
            if (!gridManager.IsAreaAvailable(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells))
            {
                FindAvailablePosition();
            }

            gridManager.OccupyArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);

            // Обновляем позицию
            Vector3 newPosition = gridManager.GridToWorldPosition(gridPosition.x, gridPosition.y);
            newPosition.x += (widthInCells - 1) * gridManager.gridSize / 2f;
            newPosition.z += (depthInCells - 1) * gridManager.gridSize / 2f;
            newPosition.y = originalY;

            transform.position = newPosition;
            targetPosition = newPosition;
        }
    }

    void UpdateBlockSize()
    {
        // Масштабируем коллайдер и визуальное представление
        if (gridManager != null)
        {
            transform.localScale = new Vector3(
                widthInCells * gridManager.gridSize,
                transform.localScale.y,
                depthInCells * gridManager.gridSize
            );
        }
        else
        {
            transform.localScale = new Vector3(widthInCells, transform.localScale.y, depthInCells);
        }

        // Обновляем коллайдер если он есть
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.size = new Vector3(1, collider.size.y, 1); // Размер всегда 1, масштабирование делается через transform
        }
    }

    // Метод для сброса позиции
    public void ResetPosition()
    {
        // Освобождаем текущую область в сетке
        if (gridManager != null)
        {
            gridManager.ReleaseArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);
        }

        targetPosition = originalPosition;
        isMovingToTarget = true;

        // Обновляем позицию в сетке после перемещения
        if (gridManager != null)
        {
            gridPosition = gridManager.WorldToGridPosition(originalPosition);
            gridManager.OccupyArea(gridPosition.x, 0, gridPosition.y, widthInCells, depthInCells);
        }
    }
}