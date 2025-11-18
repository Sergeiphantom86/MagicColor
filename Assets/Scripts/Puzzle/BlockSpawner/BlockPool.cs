using UnityEngine;
using UnityEngine.Pool;

public class BlockPool : MonoBehaviour
{
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private int _defaultPoolSize = 10;
    [SerializeField] private int _maxPoolSize = 50;
    [SerializeField] private bool _collectionCheck = true;

    private ObjectPool<Block> _pool;
    private Transform _poolParent;

    public ObjectPool<Block> Pool => _pool;

    private void Awake()
    {
        _poolParent = transform;
        CreatePool();
    }

    private void CreatePool()
    {
        _pool = new ObjectPool<Block>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: _collectionCheck,
            defaultCapacity: _defaultPoolSize,
            maxSize: _maxPoolSize
        );
    }

    private Block CreatePooledItem()
    {
        Block block = Instantiate(_blockPrefab, _poolParent);
        block.gameObject.SetActive(false);
        return block;
    }

    private void OnTakeFromPool(Block block)
    {
        block.gameObject.SetActive(true);

        // Reset block components
        var renderer = block.GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = true;

        var collider = block.GetComponent<Collider>();
        if (collider != null)
            collider.enabled = true;
    }

    private void OnReturnedToPool(Block block)
    {
        if (block != null && block.gameObject != null)
        {
            block.gameObject.SetActive(false);

            // Reset transform
            block.transform.SetParent(_poolParent);
            block.transform.localPosition = Vector3.zero;
            block.transform.localRotation = Quaternion.identity;
        }
    }

    private void OnDestroyPoolObject(Block block)
    {
        if (block != null && block.gameObject != null)
        {
            Destroy(block.gameObject);
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