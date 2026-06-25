using UnityEngine;

public class NarrowPuzzle : MonoBehaviour
{
    private int _gridSize;

    public int GridSize => _gridSize;

    private void Awake()
    {
        _gridSize = 13;
    }
}