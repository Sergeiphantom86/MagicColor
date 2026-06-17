using UnityEngine;

public class MagicSphereSpawner : BaseSpawner<MagicSphere>
{
    [SerializeField] private AudioClip _audioClip;

    private GridSystem _grid;
    private IInputHandler _input;
    private Voiceover _voiceover;

    protected override void Awake()
    {
        base.Awake();

        _input = GetComponent<IInputHandler>();
        _voiceover = GetComponent<Voiceover>();

        if (_input == null)
            Debug.LogError("InputHandler == null");
    }

    private void Start()
    {
        if (_grid == null)
            _grid = GridSystem.Instance;
    }

    private void OnEnable()
    {
        if (_input != null)
        {
            _input.OnSelected += TrySpawnAtWorldPos;
        }

        DespawnAll();
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.OnSelected -= TrySpawnAtWorldPos;
        }
    }

    private void TrySpawnAtWorldPos(Vector2 screenPos)
    {
        if (AbilitySelectionManager.Instance.HasSelection == false)
            return;

        Vector2Int origin = _grid.GetOriginFromCenter(
            center: WorldToGrid(_input.Point),
            size: Vector2Int.one);

        if (_input.Point == Vector3.zero)
            return;

        AbilitySelectionManager.Instance.Use();
        _voiceover.PlayOneShot(_audioClip);

        Vector3 spawnPos = _grid.GetWorldPosition(origin, Vector2Int.one);

        MagicSphere sphere = SpawnObjectWithCurrentIndex(spawnPos, transform);

        sphere.transform.position = spawnPos;

        _grid.PlaceObject(origin, sphere);

        AbilitySelectionManager.Instance.ClearSelection();
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = _grid.GetComponent<Grid>().WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    public void DespawnAll()
    {
        for (int i = SpawnedObjects.Count - 1; i >= 0; i--)
        {
            Despawn(SpawnedObjects[i]);
        }
    }
}