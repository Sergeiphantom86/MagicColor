using UnityEngine;
using UnityEngine.Pool;

public class PixelPool : MonoBehaviour
{
    [SerializeField] private Fragment _pixelPrefab;
    [SerializeField] private int _defaultPoolSize;
    [SerializeField] private int _maxPoolSize;
    [SerializeField] private bool _collectionCheck = true;

    private ObjectPool<Fragment> _pool;
    private Transform _poolParent;

    public ObjectPool<Fragment> Pool => _pool;

    private void Awake()
    {
        _poolParent = transform;
        _defaultPoolSize = 1000;
        _maxPoolSize = 5000;
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
        pixel.gameObject.SetActive(false);
        return pixel;
    }

    private void OnTakeFromPool(Fragment pixel)
    {
        pixel.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(Fragment pixel)
    {
        if (pixel != null && pixel.gameObject != null)
        {
            pixel.gameObject.SetActive(false);
        }
    }

    private void OnDestroyPoolObject(Fragment pixel)
    {
        if (pixel != null && pixel.gameObject != null)
        {
            Destroy(pixel.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_pool != null)
        {
            _pool.Clear();
        }
    }
}