using System;
using UnityEngine;

public interface ICollisionHandler
{
    public event Action<Collider> OnEnter;

    public event Action<Collider> OnExit;
}