using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpawner<T> : MonoBehaviour 
    where T : MonoBehaviour
{
    protected ObjectPooler<T> Pooler;
    protected List<T> SpawnedObjects;
    protected int CurrentPrefabIndex;

    protected virtual void Awake()
    {
        if (Pooler == null)
            Pooler = GetComponent<ObjectPooler<T>>();

        SpawnedObjects = new List<T>();
    }

    public virtual void Despawn(T obj)
    {
        if (SpawnedObjects.Contains(obj))
        {
            SpawnedObjects.Remove(obj);
            Pooler.Release(obj);
        }
    }

    protected T SpawnObject(Vector3 position, Transform parent = null, int prefabIndex = 0)
    {
        if (Pooler == null) 
            return null;

        T obj = Pooler.GetFromPrefab(prefabIndex);

        if (obj == null) 
            return null;

        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.gameObject.SetActive(true);

        SpawnedObjects.Add(obj);

        return obj;
    }

    protected T SpawnObjectWithCurrentIndex(Vector3 position, Transform parent = null)
    {
        return SpawnObject(position, parent, CurrentPrefabIndex);
    }

    public void SetPrefabIndex(int index)
    {
        CurrentPrefabIndex = index;
    }

    public int GetCurrentPrefabIndex() => CurrentPrefabIndex;
}