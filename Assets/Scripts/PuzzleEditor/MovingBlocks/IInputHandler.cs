using System;
using UnityEngine;

public interface IInputHandler
{
    public event Action<Vector2> OnSelected;

    public event Action<Vector2> OnMoved;

    public event Action OnThrowed;

    public Vector3 Point { get; }
}