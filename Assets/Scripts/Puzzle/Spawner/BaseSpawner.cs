using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpawner<T> : MonoBehaviour where T : MonoBehaviour
{
    protected ObjectPooler<T> _pooler;
    protected List<T> _spawnedObjects;

    protected virtual void Awake()
    {
        if (_pooler == null)
            _pooler = GetComponent<ObjectPooler<T>>();

        _spawnedObjects = new List<T>();
    }

    public virtual void Despawn(T obj)
    {
        if (_spawnedObjects.Contains(obj))
        {
            _spawnedObjects.Remove(obj);
            _pooler.Release(obj);
        }
    }

    protected T SpawnObject(Vector3 position, Transform parent = null)
    {
        if (_pooler == null) return null;

        T obj = _pooler.Get();
        if (obj == null) return null;

        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.gameObject.SetActive(true);

        _spawnedObjects.Add(obj);
        return obj;
    }
}