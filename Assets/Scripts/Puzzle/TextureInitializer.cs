using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TextureInitializer : MonoBehaviour
{
    private const int PixelSize = 1;
    private const float AlignmentMultiplier = 0.5f;
    private const float IgnoredTransparency = 0.1f;

    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private int _scaleMultiplier;
    [SerializeField] private bool _isSaveCollections = true;

    private int _totalCount;
    private Color[] _pixels;
    private Vector2 _pivot;
    private PixelPool _pixelPool;
    private ColorPrecision _precision;
    private List<Fragment> _fragmentsList;
    private Dictionary<Color, Queue<Fragment>> _fragments;

    public Dictionary<Color, Queue<Fragment>> Fragments => _fragments;
    public List<Fragment> FragmentsList => _fragmentsList;
    public int TotalCount => _totalCount;

    public event Action<int> OnInitialize;
    public event Action<List<Color>> CanPaint;

    private void Awake()
    {
        _pixelPool = GetComponent<PixelPool>();
        _fragmentsList = new List<Fragment>();
        _fragments = new Dictionary<Color, Queue<Fragment>>();
        _precision = new ColorPrecision();
    }

    public void SpawnPixelsFromTexture(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null!");
            return;
        }

        _pixels = texture.GetPixels();

        int width = texture.width;
        int height = texture.height;

        SetPivot(width, height);

        Group(width, height, _pivot);

        gameObject.transform.localScale = Vector3.one * _scaleMultiplier;

        OnInitialize?.Invoke(_fragments.Count);

        CanPaint?.Invoke(Fragments.Keys.ToList());

        if (_animator != null)
        {
            _animator.StartGame();
        }
    }

    public void ClearAllFragments()
    {
        if (_pixelPool != null && _fragmentsList != null)
        {
            _pixelPool.ReturnAllFragments(_fragmentsList);
            _fragmentsList.Clear();
        }
    }

    private void Group(int width, int height, Vector2 pivot)
    {
        Enumerable.Range(0, height)
            .SelectMany(y => Enumerable.Range(0, width), (y, x) => (x, y))
            .Select(position => (position.x, position.y, pixelColor: _pixels[position.y * width + position.x]))
            .Where(positon => positon.pixelColor.a >= IgnoredTransparency)
            .ToList()
            .ForEach(position =>
            {
                SpawnPixel(position.x, position.y, position.pixelColor, pivot);
                _totalCount++;
            });
    }

    private void SetPivot(int width, int height)
    {
        _pivot = new Vector2(width * AlignmentMultiplier, height * AlignmentMultiplier);
    }

    private void SpawnPixel(int x, int y, Color color, Vector2 pivot)
    {
        if (_pixelPool.Pool.Get().TryGetComponent(out Fragment fragment))
        {
            color = _precision.Reduce(color);

            fragment.transform.SetParent(transform != null ? transform : transform);

            fragment.transform.SetLocalPositionAndRotation(GetPosition(x, y, pivot), Quaternion.identity);
            fragment.transform.localScale = Vector3.one;
            fragment.SetColor(color);

            if (_isSaveCollections)
            {
                AddToDictionaries(color, fragment);
                fragment.TurnOnTransparency();
            }
            else
            {
                _fragmentsList.Add(fragment);
            }
        }
    }

    private void AddToDictionaries(Color color, Fragment fragment)
    {
        if (_fragments.ContainsKey(color) == false)
        {
            _fragments[color] = new Queue<Fragment>();
        }

        _fragments[color].Enqueue(fragment);
    }

    private Vector3 GetPosition(int x, int y, Vector2 pivot)
    {
        return new Vector3((x - pivot.x) * PixelSize, (y - pivot.y) * PixelSize, 0);
    }

    public Queue<Fragment> GetFragmentsByColor(Color color)
    {
        return _fragments.TryGetValue(color, out Queue<Fragment> fragments)
            ? new Queue<Fragment>(fragments)
            : new Queue<Fragment>();
    }
}