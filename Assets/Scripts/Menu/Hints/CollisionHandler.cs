using System;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public event Action<Collider> OnEnter;
    public event Action<Collider> OnExit;

    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke(other);
    }
}
