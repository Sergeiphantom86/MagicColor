using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PixelPool : MonoBehaviour
{
    [SerializeField] private Fragment _pixelPrefab;
    [SerializeField] private int _defaultPoolSize;
    [SerializeField] private int _maxPoolSize;
    [SerializeField] private bool _collectionCheck = true;

    private ObjectPool<Fragment> _pool;
    private Transform _poolParent;
    private int _scaleDefault;

    public ObjectPool<Fragment> Pool => _pool;

    private void Awake()
    {
        _poolParent = transform;
        _defaultPoolSize = 1000;
        _maxPoolSize = 5000;
        _scaleDefault = 1;

        CreatePool();
    }

    private void CreatePool()
    {
        _pool = new ObjectPool<Fragment>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: _collectionCheck,
            defaultCapacity: _defaultPoolSize,
            maxSize: _maxPoolSize
        );
    }

    private Fragment CreatePooledItem()
    {
        Fragment pixel = Instantiate(_pixelPrefab, _poolParent);
        pixel.TurnOff();
        return pixel;
    }

    private void OnTakeFromPool(Fragment pixel)
    {
        pixel.TurnOn();
    }

    public void OnReturnedToPool(Fragment pixel)
    {
        if (pixel != null && pixel.gameObject != null)
        {
            pixel.TurnOff();
            pixel.SetParent(_poolParent);
            pixel.SetLocalScale(_scaleDefault);
            pixel.SetRotation(Quaternion.identity);
        }
    }

    private void OnDestroyPoolObject(Fragment pixel)
    {
        if (pixel != null && pixel.gameObject != null)
        {
            Destroy(pixel.gameObject);
        }
    }

    public void ReturnAllFragments(List<Fragment> fragments)
    {
        if (fragments == null) return;

        foreach (Fragment fragment in fragments)
        {
            if (fragment != null)
            {
                _pool.Release(fragment);
            }
        }
    }

    private void OnDestroy()
    {
        _pool?.Clear();
    }
}