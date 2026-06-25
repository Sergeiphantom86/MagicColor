using System;
using UnityEngine;
namespace Menu.TutorialEditor
{

public class CollisionHandler : MonoBehaviour, ICollisionHandler
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
}