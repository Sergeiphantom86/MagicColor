using System.Collections.Generic;
using UnityEngine;

public class PuzzlesIdentifier : MonoBehaviour
{
    [SerializeField] private BagKey _bag;
    [SerializeField] private Lock _lock;
    [SerializeField] private Messager _hintKey;
    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private ErrorPanel _errorPanel;
    [SerializeField] private Activator _activator;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private List<WallsContainer> _wallPrefabs;
    [SerializeField] private int _index;

    private Rotator _rotation;
    private GridSystem _gridSystem;
    private BlockSpawner _blockSpawner;
    private ColorPrecision _colorPrecision;
    private ZoomChanger _zoomChanger;

    public WallsContainer CurrentContainer { get; private set; }

    private void Awake()
    {
        _rotation = GetComponent<Rotator>();
        _blockSpawner = GetComponentInChildren<BlockSpawner>();
        _colorPrecision = new ColorPrecision();
        _zoomChanger = new ZoomChanger();

        if (_wallPrefabs == null || _wallPrefabs.Count == 0)
        {
            Debug.LogError("No wall prefabs assigned", this);
            return;
        }

        if (_blockSpawner == null)
        {
            Debug.LogError("BlockSpawner == null");
            return;
        }
    }

    private void Start()
    {
        _gridSystem = GridSystem.Instance;

        if (_gridSystem == null)
        {
            Debug.LogError("GridSystem.Instance is NULL in Start");
            return;
        }

        PickUp();
    }

    private void PickUp()
    {
        if (_zoomChanger.IsMobileWithTallScreen())
        {
            _index = 3;
        }

        if (_wallPrefabs[_index] == null)
        {
            Debug.LogError($"Wall prefab is NULL at index {_index}", this);
            return;
        }

        DestroyCurrentContainer();

        CreateWalls();

        InitializeWalls();

        SetGridSize();

        _rotation.SetPositionPuzzle(0, CurrentContainer.Position.y, CurrentContainer.Position.z);
    }

    private void DestroyCurrentContainer()
    {
        if (CurrentContainer != null)
        {
            Destroy(CurrentContainer.gameObject);
            CurrentContainer = null;
        }
    }

    private void CreateWalls()
    {
        CurrentContainer = Instantiate(_wallPrefabs[_index], transform);
    }

    private void InitializeWalls()
    {
        CurrentContainer.InitializeWalls(_colorPrecision, _bag, _rotation, _hintKey, _lock, _errorPanel, _activator, _audioClip);
    }

    private void SetGridSize()
    {
        _gridSystem.SetGridSize(CurrentContainer.GridSize.x, CurrentContainer.GridSize.y);
    }
}