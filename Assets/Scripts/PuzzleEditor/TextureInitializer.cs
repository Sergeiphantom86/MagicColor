using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextureInitializer : MonoBehaviour
{
    private const int PixelSize = 1;
    private const float AlignmentMultiplier = 0.5f;
    private const float IgnoredTransparency = 0.1f;

    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private Vector3 _mobilePosition;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private bool _isSaveCollections = true;

    private int _totalCount;
    private Color[] _pixels;
    private Vector2 _pivot;
    private PixelPool _pixelPool;
    private ZoomChanger _zoomChanger;
    private ColorPrecision _precision;
    private List<Fragment> _fragmentsList;
    private Dictionary<Color, Queue<Fragment>> _fragments;

    public event Action<int> OnInitialize;

    public event Action<List<Color>> CanPaint;

    public Dictionary<Color, Queue<Fragment>> Fragments => _fragments;

    public List<Fragment> FragmentsList => _fragmentsList;

    public int TotalCount => _totalCount;

    private void Awake()
    {
        _pixelPool = GetComponent<PixelPool>();
        _zoomChanger = new ZoomChanger();
        _fragmentsList = new List<Fragment>();
        _fragments = new Dictionary<Color, Queue<Fragment>>();
        _precision = new ColorPrecision();
    }

    public Queue<Fragment> GetFragmentsByColor(Color color)
    {
        return _fragments.TryGetValue(color, out Queue<Fragment> fragments)
            ? new Queue<Fragment>(fragments)
            : new Queue<Fragment>();
    }

    public void SpawnPixelsFromTexture(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null!");
            return;
        }

        EditMobile();

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

    private void EditMobile()
    {
        if (_zoomChanger.IsMobileWithTallScreen() && SceneManager.GetActiveScene().name == "Menu")
        {
            _scaleMultiplier = 25;
        }

        if (_zoomChanger.IsMobileWithTallScreen() && SceneManager.GetActiveScene().name != "Menu")
        {
            transform.position = _mobilePosition;
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

        fragment.TurnOnTransparency();
    }

    private Vector3 GetPosition(int x, int y, Vector2 pivot)
    {
        return new Vector3((x - pivot.x) * PixelSize, (y - pivot.y) * PixelSize, 0);
    }
}