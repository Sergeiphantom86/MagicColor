using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PixelPool), typeof(Voiceover))]
public class PixelSpawner : MonoBehaviour
{
    private float _pixelSize;
    private PixelPool _pixelPool;
    private List<Fragment> _pixels;

    private void Awake()
    {
        _pixelSize = 2;
        _pixels = new List<Fragment>();
        _pixelPool = GetComponent<PixelPool>();

        if (_pixelPool == null)
        {
            Debug.Log($"PixelPool Не назначен!!!{this}");
        }
    }

    public List<Fragment> CreatePixels(Dictionary<Color, List<Vector3>> colorGroups, Vector2 centerOffset)
    {
        foreach (var colorGroup in colorGroups)
        {
            SpawnColorGroup(colorGroup.Key, colorGroup.Value, centerOffset);
        }

        return _pixels;
    }

    public void Clear()
    {
        if (_pixels == null || _pixelPool == null) return;

        foreach (Fragment pixel in _pixels)
        {
            if (pixel != null)
            {
                _pixelPool.Pool.Release(pixel);
            }
        }

        _pixels.Clear();
    }

    private void SpawnColorGroup(Color color, List<Vector3> positions, Vector2 centerOffset)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            CreatePixel(GetPositions(positions[i], centerOffset), color);
        }
    }

    private void CreatePixel(Vector3 position, Color color)
    {
        if (_pixelPool == null)
        {
            Debug.LogError("PixelPool is not assigned!");
            return;
        }

        Fragment pixel = _pixelPool.Pool.Get();

        if (pixel != null)
        {
            ConfigurePixel(pixel, position, color);

            _pixels.Add(pixel);
        }
    }

    private void ConfigurePixel(Fragment pixel, Vector3 position, Color color)
    {
        if (pixel == null) return;

        pixel.SetParent(transform);
        pixel.SetPosition(position);
        pixel.SetLocalScale(_pixelSize);
        pixel.SetRotation(Quaternion.identity);

        pixel.SetColor(color);
    }

    private Vector3 GetPositions(Vector3 texturePosition, Vector2 centerOffset)
    {
        Vector3 worldPosition = transform.position;

        worldPosition.x += GetPositionOnCoordinate(texturePosition.x, centerOffset.x);
        worldPosition.y += GetPositionOnCoordinate(texturePosition.y, centerOffset.y);

        return worldPosition;
    }

    private float GetPositionOnCoordinate(float texturePosition, float centerOffset)
    {
        return (texturePosition - centerOffset) * _pixelSize;
    }
}