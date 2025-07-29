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

    private Camera _targetCamera;
    private ImageAnalyzer _imageAnalyzer;
    private Dictionary<Color, Queue<Fragment>> _spawnedFragments;
    private Vector3 _canvasNormal;
    private Vector3 _canvasRight;
    private Vector3 _canvasUp;
    private Vector3 _offsetFromCenter;
    private float _pixelSize;

    public int TotalCount { get; private set; }

    public event Action OnStart;

    private void Awake()
    {
        _targetCamera = Camera.main;
        _spawnedFragments = new Dictionary<Color, Queue<Fragment>>();
        _imageAnalyzer = GetComponent<ImageAnalyzer>();
        _pixelSize = 2.0202f;
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
        return _spawnedFragments.TryGetValue(color, out var fragments)
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


        foreach (var colorGroup in colorGroups)
        {
            _spawnedFragments[colorGroup.Key] = new Queue<Fragment>(
                colorGroup.Value.Select(pixelPosition =>
                    GetFragment(pixelPosition, colorGroup.Key))
            );
        }

        if (_animator != null)
        {
            _animator.StartGame();
        }

        OnStart?.Invoke();
    }

    private Fragment GetFragment(Vector3 pixelPosition, Color pixelColor)
    {
        _prefab = Instantiate(_prefab);

        _prefab.transform.position = ConvertPixelToWorldPosition(pixelPosition);
        _prefab.transform.rotation = Quaternion.LookRotation(_canvasNormal);
        _prefab.transform.SetParent(_transformParent);
        _prefab.transform.localScale = Vector3.one * _scale;

        _prefab.SetColor(pixelColor);
        _prefab.TurnOnTransparency();

        TotalCount++;

        return _prefab;
    }

    private Vector3 ConvertPixelToWorldPosition(Vector3 pixelPosition)
    {
        _offsetFromCenter = pixelPosition - _imageAnalyzer.Pivot;

        _offsetFromCenter *= 0.05f;

        return _transformParent.position + (_canvasRight * _offsetFromCenter.x + _canvasUp * _offsetFromCenter.y) * _pixelSize;
    }
}