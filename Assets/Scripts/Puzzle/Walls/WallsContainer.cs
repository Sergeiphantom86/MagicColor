using System.Collections.Generic;
using UnityEngine;

public class WallsContainer : MonoBehaviour
{
    [SerializeField] private Bag _bag;
    [SerializeField] private Activator _activator;
    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private List<IColorable> _colorables;

    private List<WallEngine> _walls;
    private WallEngine _wall;
    private ColorPrecision _colorPrecision;

    private void Awake()
    {
        _walls = new List<WallEngine>();
        _colorPrecision = new ColorPrecision();
    }

    private void Start()
    {
        InitializeWalls(_colorPrecision, _activator, _bag);
    }

    private void InitializeWalls(ColorPrecision colorPrecision, Activator activator, Bag bag)
    {
        foreach (Transform child in transform)
        {
            _wall = child.GetComponent<WallEngine>();

            if (_wall != null)
            {
                _walls.Add(_wall);

                _wall.Initialize(colorPrecision, activator, bag);
            }
        }
    }
}