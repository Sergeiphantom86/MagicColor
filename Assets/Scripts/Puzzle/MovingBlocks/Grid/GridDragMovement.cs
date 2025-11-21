using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GridDragMovement : MonoBehaviour
{
    [SerializeField] private BlockSpawner _blockSpawner;
    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private ParticleSystemPool _particleSystemPool;

    private float _moveDuration;
    private Transform _transform;

    private Vector3 _lastTouchWorldPosition;
    private Vector3 _accumulatedWorldDisplacement;
    private Vector2Int _currentGridPosition;
    private Tween _currentTween;

    private void Awake()
    {
        _moveDuration = 0.15f;
        _transform = transform;
    }

    private void Start()
    {
        InitializeGridSystem();
        PositionAllBlocks();
    }

    public void BeginInteraction(Vector3 touchPosition, Vector3 originalPosition)
    {
        _currentGridPosition = _gridSystem.WorldToGridPosition(originalPosition);
        _lastTouchWorldPosition = CalculateTouchWorldPosition(touchPosition, originalPosition);
        _accumulatedWorldDisplacement = Vector3.zero;
    }

    public void ProcessInput(Vector3 touchPosition, Vector3 originalPosition, Voiceover voiceover, AudioClip audioClip)
    {
        Vector3 worldTouchPoint = CalculateTouchWorldPosition(touchPosition, originalPosition);
        Vector3 delta = worldTouchPoint - _lastTouchWorldPosition;

        _lastTouchWorldPosition = worldTouchPoint;
        _accumulatedWorldDisplacement += delta;

        float cellSize = _gridSystem.CellSize;

        if (_accumulatedWorldDisplacement.sqrMagnitude > cellSize)
        {
            AttemptShift(_accumulatedWorldDisplacement, voiceover, audioClip);
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(ParticleSystem effect, ParticleSystemPool pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Pool.Release(effect);
    }

    private void AttemptShift(Vector3 accumulatedWorldDisplacement, Voiceover voiceover, AudioClip audioClip)
    {
        Vector2Int shiftDirection = GetShiftDirection(accumulatedWorldDisplacement);
        Vector2Int newGridPos = CalculateNewGridPosition(shiftDirection);

        if (CanShiftToPosition(newGridPos) == false) return;

        _gridSystem.ClearCell(_currentGridPosition);

        voiceover.Play(audioClip);

        ParticleSystem particleSystem = _particleSystemPool.Pool.Get();
        CraeteEffect(particleSystem);

        StartCoroutine(ReturnToPoolAfterDelay(particleSystem, _particleSystemPool, particleSystem.main.duration));

        ExecuteShift(newGridPos);
    }

    private void CraeteEffect(ParticleSystem particleSystem)
    {
        particleSystem.transform.position = transform.position;
        particleSystem.transform.localScale = Vector3.one * 20;
        particleSystem.transform.rotation = Quaternion.identity;
        particleSystem.Play();
    }

    private Vector2Int GetShiftDirection(Vector3 accumulatedWorldDisplacement)
    {
        return Mathf.Abs(accumulatedWorldDisplacement.x) >= Mathf.Abs(accumulatedWorldDisplacement.z) ?
            new Vector2Int(_accumulatedWorldDisplacement.x > 0 ? 1 : -1, 0) :
            new Vector2Int(0, _accumulatedWorldDisplacement.z > 0 ? 1 : -1);
    }

    private Vector2Int CalculateNewGridPosition(Vector2Int shiftDirection) =>
        ClampToGridBounds(_currentGridPosition + shiftDirection);

    private bool CanShiftToPosition(Vector2Int newGridPosition) =>
        newGridPosition != _currentGridPosition && _gridSystem.IsCellEmpty(newGridPosition);

    private void ExecuteShift(Vector2Int newGridPosition)
    {
        PositionAtCell(newGridPosition);
        _accumulatedWorldDisplacement = Vector3.zero;
    }

    private void PositionAllBlocks()
    {
        if (_blockSpawner != null)
        {
            foreach (var block in _blockSpawner.SpawnedBlocks)
            {
                Vector2Int gridPosition = ClampToGridBounds(_gridSystem.WorldToGridPosition(block.transform.position));
                block.transform.position = _gridSystem.GridToWorldPosition(gridPosition);
                _gridSystem.UpdateCell(gridPosition, block.gameObject);
            }
        }
    }

    private void InitializeGridSystem()
    {
        _gridSystem = GridSystem.Instance;
        if (_gridSystem == null)
            Debug.LogError("GridSystem not found! Add GridSystem component to an object.");
    }

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

    private Vector3 CalculateTouchWorldPosition(Vector3 touchPosition, Vector3 originalPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        return new Plane(Vector3.up, originalPosition).Raycast(ray, out float distance) ?
            ray.GetPoint(distance) :
            originalPosition;
    }

    private Vector2Int ClampToGridBounds(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, _gridSystem.GridSizeX - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, _gridSystem.GridSizeY - 1);
        return gridPosition;
    }
}