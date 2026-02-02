using System.Collections.Generic;
using UnityEngine;

public class PuzzlesIdentifier : MonoBehaviour
{
    [SerializeField] private Bag _bag;
    [SerializeField] private Lock _lock;
    [SerializeField] private HintKey _hintKey;
    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private EffectsHandler _effectsHandler;
    [SerializeField] private ErrorPanel _errorPanel;
    [SerializeField] private Activator _activator;
    [SerializeField] private AudioClip _audioClip;

    [SerializeField] private List<WallsContainer> _wallPrefabs;
    [SerializeField] private int _index;

    private GridSystem _gridSystem;
    private Rotator _rotation;
    private ColorPrecision _colorPrecision;

    public WallsContainer CurrentContainer { get; private set; }

    private void Awake()
    {
        _rotation = GetComponent<Rotator>();
        _colorPrecision = new ColorPrecision();

        _gridSystem = GridSystem.Instance;

        if (_wallPrefabs == null || _wallPrefabs.Count == 0)
        {
            Debug.LogError("No wall prefabs assigned", this);
            return;
        }

        PickUp();
    }


    private void PickUp()
    {
        _index = TryGetSuitableIndex();

        if (_wallPrefabs[_index] == null)
        {
            Debug.LogError($"Wall prefab is NULL at index {_index}", this);
            return;
        }

        DestroyCurrentContainer();

        CurrentContainer = Instantiate(_wallPrefabs[_index], transform);

        CurrentContainer.InitializeWalls(_colorPrecision,_bag,_rotation,_hintKey,_lock,_effectsHandler,_errorPanel,_activator, _audioClip);

        _gridSystem.SetGridSize(
            CurrentContainer.GridSize.x,
            CurrentContainer.GridSize.y
        );

        _rotation.SetPositionPuzzle(
            CurrentContainer.Position.x,
            CurrentContainer.Position.y,
            CurrentContainer.Position.z
        );
    }


    private void DestroyCurrentContainer()
    {
        if (CurrentContainer != null)
        {
            Destroy(CurrentContainer.gameObject);
            CurrentContainer = null;
        }
    }


    private int TryGetSuitableIndex()
    {
        if (_index < 0)
        {
            return  0;
        }
        else if (_index >= _wallPrefabs.Count)
        {
            return  0;
        }

        return _index;
    }
}