using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private RotatorX _rotation;

    private void Awake()
    {
        _rotation = GetComponent<RotatorX>();
    }

    public void Return()
    {
        _rotation.Return();
    }

    public void StartRotation()
    {
        _rotation.StartRotation();
    }
}