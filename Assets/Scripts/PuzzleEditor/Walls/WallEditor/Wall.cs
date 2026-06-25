using UnityEngine;
namespace PuzzleEditor.Walls.WallEditor
{

public class Wall : ColorableObject, IUnblocker, IPointer
{
    private bool _isBlocked;
    private float _height;

    public Vector3 EndPoint { get; private set; }

    public Vector3 MiddlePoint { get; private set; }

    public Transform CenterFence => transform;

    public bool IsBlocked => _isBlocked;

    public float Height => _height;

    private void Awake()
    {
        InitializeComponents();
    }

    public void SetPosition(Vector3 middlePoint, Vector3 endPoint)
    {
        EndPoint = endPoint;

        MiddlePoint = middlePoint;
    }

    public float GetAngleY()
    {
        return transform.rotation.eulerAngles.y;
    }

    public void Unblock()
    {
        _isBlocked = false;
    }

    public void Block()
    {
        _isBlocked = true;
    }
}
}