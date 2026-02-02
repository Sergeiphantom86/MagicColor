using System;
using UnityEngine;

public class KeyCollector : MonoBehaviour
{
   private ICollisionHandler _collisionHandler;

    public event Action<Key> OnAdded;

    private void Awake()
    {
        _collisionHandler = GetComponent<ICollisionHandler>();
    }

    private void OnEnable()
    {
        _collisionHandler.OnEnter += Add;
    }

    private void OnDisable()
    {
        _collisionHandler.OnEnter -= Add;
    }

    private void Add(Collider collider)
    {
        if (collider.TryGetComponent(out Key key))
        {
            key.gameObject.SetActive(false);
            OnAdded?.Invoke(key);
        }
    }
}