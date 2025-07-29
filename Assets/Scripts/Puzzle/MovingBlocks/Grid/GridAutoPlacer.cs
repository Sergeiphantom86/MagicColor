using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GridSystem))]
public class GridAutoPlacer : MonoBehaviour
{
    [SerializeField] private BlocksContainer _blocksContainer;

    GridSystem _gridSystem;

    private void Awake()
    {
        _gridSystem = GetComponent<GridSystem>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        PlaceAllObjectsInGrid();
    }

    private void PlaceAllObjectsInGrid()
    {
        foreach (var block in _blocksContainer.Blocks)
        {
            SnapObjectToGrid(block.transform);
        }
    }

    public void SnapObjectToGrid(Transform objectTransform)
    {
        Vector2Int gridPosition = _gridSystem.WorldToGridPosition(objectTransform.position);
        gridPosition = ApplyGridConstraints(gridPosition);

        Vector3 worldPosition = _gridSystem.GridToWorldPosition(gridPosition);
        objectTransform.position = worldPosition;

        _gridSystem.UpdateCell(gridPosition, objectTransform.gameObject);
    }

    private Vector2Int ApplyGridConstraints(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, _gridSystem.GridSizeX - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, _gridSystem.GridSizeY - 1);

        return gridPosition;
    }
}