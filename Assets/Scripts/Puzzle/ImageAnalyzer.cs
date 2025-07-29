using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class ImageAnalyzer : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    private bool _ignoreTransparent;
    private float _ignoredAlpha;
    private Texture2D _texture;
    private IColorPrecision _colorPrecision;
    private Dictionary<Color, List<Vector3>> _colorGroups;

    public Vector3 Pivot => _sprite.pivot;

    public event Action<List<Color>> CanPaint;
    public event Action<Dictionary<Color, List<Vector3>>> CanRender;

    private void Awake()
    {
        _ignoreTransparent = true;
        _ignoredAlpha = 0.1f;
        _colorPrecision = new ColorPrecision();
    }

    public void AnalyzeTexture(Sprite sprite)
    {
        ValidateSprite(sprite);
        
        _texture = sprite.texture;
        _sprite = sprite;
        _colorGroups = GetDictionaryColorsAndPositions(_texture);

        CanPaint?.Invoke(_colorGroups.Keys.ToList());
        CanRender?.Invoke(_colorGroups);
    }

    public string GetNameSprite()
    {
        return _sprite.name;
    }

    private Dictionary<Color, List<Vector3>> GetDictionaryColorsAndPositions(Texture2D texture)
    {
        return GroupPixels(FilterPixels(GetPixelData(texture)));
    }

    private Dictionary<Color, List<Vector3>> GroupPixels(IEnumerable<PixelInfo> pixels)
    {
        return pixels.GroupBy(
                pixelInfo => _colorPrecision.Reduce(pixelInfo.Color),
                pixelInfo => pixelInfo.Position).ToDictionary(group => group.Key, group => group.ToList());
    }

    private IEnumerable<PixelInfo> FilterPixels(IEnumerable<PixelInfo> pixels)
    {
        return pixels.Where(pixelInfo => _ignoreTransparent == false || pixelInfo.Color.a >= _ignoredAlpha);
    }

    private IEnumerable<PixelInfo> GetPixelData(Texture2D texture)
    {
        int width = texture.width;

        return texture.GetPixels().Select((color, index) => new PixelInfo
        {
            Color = color,
            Position = new Vector3(index % width, index / width)
        });
    }

    private bool ValidateSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Исходная текстура не назначена!");
            return _sprite;
        }

        if (sprite.texture.isReadable == false)
        {
            Debug.LogError("Текстура не читается! Включите чтение/запись в настройках импорта.");
            return _sprite;
        }

        return true;
    }
}