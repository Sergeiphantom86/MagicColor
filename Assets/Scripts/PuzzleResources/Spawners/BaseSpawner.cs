using System.Collections.Generic;
using PuzzleResources.ObjectPool;
using UnityEngine;

namespace PuzzleResources.Spawners
{
    public abstract class BaseSpawner<T> : MonoBehaviour
    where T : MonoBehaviour
    {
        private ObjectPooler<T> _pooler;
        private List<T> _spawnedObjects;
        private int _currentPrefabIndex;

        public ObjectPooler<T> Pooler => Pooler;

        public List<T> SpawnedObjects => _spawnedObjects;

        public int CurrentPrefabIndex => _currentPrefabIndex;

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

        protected T SpawnObject(Vector3 position, Transform parent = null, int prefabIndex = 0)
        {
            if (_pooler == null)
            return null;

            T obj = _pooler.GetFromPrefab(prefabIndex);

            if (obj == null)
            return null;

            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.gameObject.SetActive(true);

            _spawnedObjects.Add(obj);

            return obj;
        }

        protected T SpawnObjectWithCurrentIndex(Vector3 position, Transform parent = null)
        {
            return SpawnObject(position, parent, _currentPrefabIndex);
        }

        public void SetPrefabIndex(int index)
        {
            _currentPrefabIndex = index;
        }
    }
}