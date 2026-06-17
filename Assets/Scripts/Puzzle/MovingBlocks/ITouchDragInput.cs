using System;
using UnityEngine;

public interface ITouchDragInput
{
    public event Action<Vector2> OnTouchClick;

    public event Action<Vector2> OnTouchDrag;

    public event Action OnDropped;

    public bool IsSelected { get; }

    public void ThrowOff();
}