using UnityEngine;
using UnityEngine.Pool;

public class BlockPool : MonoBehaviour
{
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private bool _collectionCheck;

    private int _maxPoolSize;
    private int _defaultPoolSize;
    private ObjectPool<Block> _pool;

    public ObjectPool<Block> Pool => _pool;

    private void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        _maxPoolSize = 50;
        _defaultPoolSize = 10;

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
        Block block = Instantiate(_blockPrefab, transform);

        return block;
    }

    private void OnTakeFromPool(Block block)
    {
        if (block != null)
        {
            block.gameObject.SetActive(true);
        }
    }

    private void OnReturnedToPool(Block block)
    {
        if (block != null)
        {
            block.gameObject.SetActive(false);
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
        _pool?.Clear();
    }
}