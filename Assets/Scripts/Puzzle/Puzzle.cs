using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private Rotator _rotation;

    private void Awake()
    {
        _rotation = GetComponent<Rotator>();
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