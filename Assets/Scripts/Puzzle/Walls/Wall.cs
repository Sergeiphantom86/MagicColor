using UnityEngine;

public class Wall : ColorableObject
{
    private Point _point;
    private bool _isBlocked;

    public Transform Point => _point.transform;
    public Transform CenterFence => transform;
    public bool IsBlocked => _isBlocked;
    
    private void Awake()
    {
        InitializeComponents();

        _point = GetComponentInChildren<Point>();
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