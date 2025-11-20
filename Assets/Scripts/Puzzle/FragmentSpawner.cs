using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ImageAnalyzer))]
public class FragmentSpawner : MonoBehaviour
{
    [SerializeField] private float _spacingFactor;
    [SerializeField] private float _screenCoverage;
    [SerializeField] private float _maxPuzzleSize;
    [SerializeField] private Fragment _prefab;
    [SerializeField] private Transform _transformParent;
    [SerializeField] private AnimatorPuzzle _animator;

    private int _totalCount;
    private float _pixelSize;
    private Vector3 _canvasUp;
    private Vector3 _canvasRight;
    private Vector3 _canvasNormal;
    private Camera _targetCamera;
    private Dictionary<Color, Queue<Fragment>> _fragments;
    private PuzzleSizeCalculator _sizeCalculator;
    private ImageAnalyzer _imageAnalyzer;

    public int TotalCount => _totalCount;
    public Dictionary<Color, Queue<Fragment>> Fragments => _fragments;

    public event Action OnStart;

    private void Awake()
    {
        _targetCamera = Camera.main;
        _fragments = new Dictionary<Color, Queue<Fragment>>();
        _imageAnalyzer = GetComponent<ImageAnalyzer>();

        InitializeCanvasOrientation();
        InitializeSizeCalculator();
    }

    private void OnEnable()
    {
        _imageAnalyzer.CanRender += SpawnAllFragments;
    }

    private void OnDisable()
    {
        _imageAnalyzer.CanRender -= SpawnAllFragments;
    }

    public Queue<Fragment> GetFragmentsByColor(Color color)
    {
        return _fragments.TryGetValue(color, out var fragments)
            ? new Queue<Fragment>(fragments)
            : new Queue<Fragment>();
    }

    private void InitializeSizeCalculator()
    {
        _sizeCalculator = new PuzzleSizeCalculator(_targetCamera, _screenCoverage, _maxPuzzleSize);
    }

    private void InitializeCanvasOrientation()
    {
        _canvasNormal = _targetCamera.transform.forward;
        _canvasRight = _targetCamera.transform.right;
        _canvasUp = _targetCamera.transform.up;
    }

    private void CalculatePixelSize()
    {
        var sizeData = _sizeCalculator.CalculatePuzzleSize(_imageAnalyzer.TextureWidth, _imageAnalyzer.TextureHeight);
        _pixelSize = sizeData.PixelSize;
    }

    private void SpawnAllFragments(Dictionary<Color, List<Vector3>> colorGroups)
    {
        CalculatePixelSize();

        if (_imageAnalyzer == null)
            throw new ArgumentNullException(nameof(_imageAnalyzer), "ImageAnalyzer не назначен!");
        if (_animator == null)
            throw new ArgumentNullException(nameof(_animator), "AnimatorPuzzle не назначен!");

        GetFragments(colorGroups);

        _animator.StartGame();
        OnStart?.Invoke();
    }

    private void GetFragments(Dictionary<Color, List<Vector3>> colorGroups)
    {
        foreach (var colorGroup in colorGroups)
        {
            _fragments[colorGroup.Key] = new Queue<Fragment>(
                colorGroup.Value.Select(pixelPosition =>
                    GetFragment(pixelPosition, colorGroup.Key))
            );
        }
    }

    private Fragment GetFragment(Vector3 pixelPosition, Color pixelColor)
    {
        Fragment fragment = Instantiate(_prefab);
        fragment.transform.SetParent(_transformParent);

        fragment.SetPosition(ConvertPixelToWorldPosition(pixelPosition));
        fragment.SetRotation(Quaternion.LookRotation(_canvasNormal));

        fragment.SetLocalScale(_pixelSize * _spacingFactor);

        fragment.SetColor(pixelColor);
        fragment.TurnOnTransparency();

        _totalCount++;

        return fragment;
    }

    private Vector3 ConvertPixelToWorldPosition(Vector3 pixelPosition)
    {
        return GetPosition(pixelPosition);
    }

    private Vector3 GetPosition(Vector3 pixelPosition)
    {
        return _transformParent.position + GetOffsetFromCenter(pixelPosition);
    }

    private Vector3 GetOffsetFromCenter(Vector3 pixelPosition)
    {
        Vector2 offsetFromCenter = (Vector2)(pixelPosition - _imageAnalyzer.Pivot);
        offsetFromCenter *= _pixelSize;

        return _canvasRight * offsetFromCenter.x + _canvasUp * offsetFromCenter.y;
    }
}