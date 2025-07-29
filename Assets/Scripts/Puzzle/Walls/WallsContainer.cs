using UnityEngine;
using System.Collections.Generic;

public class WallsContainer : MonoBehaviour
{
    [SerializeField] private Activator _activator;
    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private List<IColorable> _colorables;

    private List<WallEngine> _walls;
    WallEngine _wall;

    private void Awake()
    {
        _walls = new List<WallEngine>();

        InitializeWalls();
    }

    private void InitializeWalls()
    {
        foreach (Transform child in transform)
        {
            _wall = child.GetComponent<WallEngine>();

            if (_wall != null)
            {
                _walls.Add(_wall);

                _wall.Initialize(new ColorPrecision(), _activator);
                _wall.SetStartPosition();
            }
        }
    }
}