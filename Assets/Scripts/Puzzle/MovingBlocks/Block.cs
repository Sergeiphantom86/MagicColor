using System;
using System.Collections;
using UnityEngine;

public class Block : ColorableObject, IDestroyable
{
    private Vector2Int _gridPosition;
    private PathMover _pathMover;
    private InkSpawner _inkSpawner;
    private Renderer _renderer;
    
    public event Action<Block> OnDestroyed;

    private void Awake()
    {
        _pathMover = GetComponent<PathMover>();
        _inkSpawner = GetComponentInChildren<InkSpawner>();
        _renderer = GetComponent<Renderer>();
    }

    public void Destroy(Transform waypoint, Transform endPoint)
    {
        _pathMover.Move(waypoint, endPoint, ExecuteDestruction);
    }

    private void ExecuteDestruction()
    {
        OnDestroyed?.Invoke(this);

        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ClearCell(_gridPosition);
        }

        _inkSpawner.ActivateInkDrops(GetColor());
       StartCoroutine(Wait());
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        _gridPosition = gridPosition;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.3f);

        _renderer.enabled = false;
    }
}