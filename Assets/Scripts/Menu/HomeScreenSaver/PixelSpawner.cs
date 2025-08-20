using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PixelSpawner : MonoBehaviour
{
    [SerializeField] private Fragment _pixelPrefab;
    [SerializeField] private int _defaultPoolSize = 1000;
    [SerializeField] private int _maxPoolSize = 5000;
    [SerializeField] private bool _collectionCheck = true;

    private float _spacing = 0.045f;
    private float _pixelSize = 20f;
    private Transform _transform;
    private List<Fragment> _activePixels;
    private ObjectPool<Fragment> _pixelPool;

    public List<Fragment> Pixels => new List<Fragment>(_activePixels);

    private void Awake()
    {
        _transform = transform;
        _activePixels = new List<Fragment>();

        _pixelPool = new ObjectPool<Fragment>(
            CreatePooledItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            _collectionCheck,
            _defaultPoolSize,
            _maxPoolSize
        );
    }

    private Fragment CreatePooledItem()
    {
        Fragment pixel = Instantiate(_pixelPrefab, _transform);
        pixel.gameObject.SetActive(false);
        return pixel;
    }

    private void OnTakeFromPool(Fragment pixel)
    {
        pixel.gameObject.SetActive(true);
        _activePixels.Add(pixel);
    }

    private void OnReturnedToPool(Fragment pixel)
    {
        if (pixel != null)
        {
            pixel.gameObject.SetActive(false);
            _activePixels.Remove(pixel);

            pixel.transform.SetParent(_transform);
            pixel.transform.localPosition = Vector3.zero;
            pixel.transform.localRotation = Quaternion.identity;
            pixel.transform.localScale = Vector3.one;
        }
    }

    private void OnDestroyPoolObject(Fragment pixel)
    {
        if (pixel != null)
        {
            Destroy(pixel.gameObject);
        }
    }

    public void SpawnPixels(Dictionary<Color, List<Vector3>> colorGroups, Vector2 centerOffset)
    {
        Clear();

        foreach (var colorGroup in colorGroups)
        {
            SpawnColorGroup(colorGroup.Key, colorGroup.Value, centerOffset);
        }
    }

    private void SpawnColorGroup(Color color, List<Vector3> positions, Vector2 centerOffset)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 position = CalculatePosition(positions[i], centerOffset);
            SpawnPixel(position, color);
        }
    }

    private void SpawnPixel(Vector3 position, Color color)
    {
        Fragment pixel = _pixelPool.Get();

        if (pixel != null)
        {
            pixel.transform.position = position;
            pixel.transform.localScale = new Vector3(_pixelSize, _pixelSize, 1);
            pixel.SetColor(color);
        }
    }

    private Vector3 CalculatePosition(Vector3 texturePosition, Vector2 centerOffset)
    {
        float posX = (texturePosition.x - centerOffset.x) * _pixelSize * _spacing;
        float posY = (texturePosition.y - centerOffset.y) * _pixelSize * _spacing;

        Vector3 worldPosition = _transform.position;
        worldPosition.x += posX;
        worldPosition.y += posY;

        return worldPosition;
    }

    public void Clear()
    {
        List<Fragment> pixelsToClear = new List<Fragment>(_activePixels);

        foreach (Fragment pixel in pixelsToClear)
        {
            if (pixel != null)
            {
                _pixelPool.Release(pixel);
            }
        }

        _activePixels.Clear();
    }

    private void OnDestroy()
    {
        if (_pixelPool != null)
        {
            _pixelPool.Clear();
        }
    }
}