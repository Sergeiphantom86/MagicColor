using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ImageAnalyzer), typeof(Image), typeof(PixelSpawner))]
public class PixelRenderer : MonoBehaviour
{
    private PixelSpawner _pixelSpawner;
    private ImageAnalyzer _analyzer;
    private Image _image;
    private Vector2 _centerOffset;
    private List<Fragment> _fragments;

    public List<Fragment> Fragments => _fragments;

    public event Action OnPixelsRendered;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _analyzer = GetComponent<ImageAnalyzer>();
        _pixelSpawner = GetComponent<PixelSpawner>();
    }

    private void OnEnable()
    {
        if (_analyzer != null)
        {
            _analyzer.CanRender += Create;
        }
    }

    private void OnDisable()
    {
        if (_analyzer != null)
        {
            _analyzer.CanRender -= Create;
        }
    }

    public void Create(Dictionary<Color, List<Vector3>> colorGroups)
    {
        if (_pixelSpawner == null)
        {
            Debug.LogError("Pixel spawner is not assigned!");
            return;
        }

        CalculateCenterOffset(_image.sprite);
        _pixelSpawner.SpawnPixels(colorGroups, _centerOffset);
        _fragments = _pixelSpawner.Pixels;
        OnPixelsRendered?.Invoke();
    }

    private void CalculateCenterOffset(Sprite sprite)
    {
        _centerOffset = GetCenter(sprite) / 2f - (Vector2)_analyzer.Pivot;
    }

    private Vector2 GetCenter(Sprite sprite)
    {
        return new Vector2(sprite.texture.width, sprite.texture.height);
    }
}