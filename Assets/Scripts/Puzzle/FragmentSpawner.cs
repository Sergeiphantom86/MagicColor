using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(ImageAnalyzer))]
public class FragmentSpawner : MonoBehaviour
{
    [SerializeField] private Fragment _prefab;
    [SerializeField] private float _scale;
    [SerializeField] private Transform _transformParent;
    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private Canvas _canvas;

    private int _totalCount;
    private Vector3 _canvasUp;
    private Vector3 _canvasRight;
    private Camera _targetCamera;
    private Vector3 _canvasNormal;
    private Vector3 _offsetFromCenter;
    private ImageAnalyzer _imageAnalyzer;
    private Dictionary<Color, Queue<Fragment>> _fragments;

    public int TotalCount => _totalCount;
    public Dictionary<Color, Queue<Fragment>> Fragments => _fragments;

    public event Action OnStart;

    private void Awake()
    {
        _targetCamera = Camera.main;
        _fragments = new Dictionary<Color, Queue<Fragment>>();
        _imageAnalyzer = GetComponent<ImageAnalyzer>();
        _scale = 4.4f;
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

    private void InitializeCanvasOrientation()
    {
        _canvasNormal = _targetCamera.transform.forward;
        _canvasRight = _targetCamera.transform.right;
        _canvasUp = _targetCamera.transform.up;
    }

    private void SpawnAllFragments(Dictionary<Color, List<Vector3>> colorGroups)
    {
        InitializeCanvasOrientation();

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
        _prefab = Instantiate(_prefab);

        _prefab.transform.SetParent(_transformParent);

        _prefab.SetPosition(ConvertPixelToWorldPosition(pixelPosition));
        _prefab.SetRotation(Quaternion.LookRotation(_canvasNormal));
        _prefab.SetLocalScale(_scale);

        _prefab.SetColor(pixelColor);
        _prefab.TurnOnTransparency();

        _totalCount++;

        return _prefab;
    }

    private Vector3 ConvertPixelToWorldPosition(Vector3 pixelPosition)
    {
        return GetPosition(pixelPosition);
    }

    private Vector3 GetPosition(Vector3 pixelPosition)
    {
        return _transformParent.position + GetOffsetPreviousPixel(pixelPosition);
    }

    private Vector3 GetOffsetPreviousPixel(Vector3 pixelPosition)
    {
        return _canvasRight * GetOffsetFromCenter(pixelPosition).x + _canvasUp * GetOffsetFromCenter(pixelPosition).y;
    }

    private Vector3 GetOffsetFromCenter(Vector3 pixelPosition)
    {
        _offsetFromCenter = pixelPosition - _imageAnalyzer.Pivot;

        _offsetFromCenter *= 0.14f;

        return _offsetFromCenter;
    }
}