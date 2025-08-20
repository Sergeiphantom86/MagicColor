using System;
using System.Collections;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

[RequireComponent(typeof(Renderer), typeof(Indicator), typeof(Rigidbody))]
public class ColorCollisionHandler : MonoBehaviour
{
    [SerializeField] private EffectsHandler effectsHandler;

    private float _delay;
    private Renderer _renderer;
    private Activator _activator;
    private Coroutine _coroutine;
    private WaitForSeconds _waitForSeconds;
    private IColorPrecision _colorPrecision;
    private InkSpawner _inkSpawner;
    private Indicator _indicator;

    public event Action<Block> IsTouch;
    public event Action<Collider> TouchEnded;

    private void Awake()
    {
        _delay = 0.1f;
        _renderer = GetComponent<Renderer>();
        _indicator = GetComponent<Indicator>();
        _inkSpawner = GetComponentInChildren<InkSpawner>();
        _waitForSeconds = new WaitForSeconds(_delay);
    }

    private void OnEnable()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_indicator == null) _indicator = GetComponent<Indicator>();
        if (_inkSpawner == null) _inkSpawner = GetComponentInChildren<InkSpawner>();
    }

    public void Initialize(IColorPrecision colorPrecision, Activator activator)
    {
        _colorPrecision = colorPrecision;
        _activator = activator;

        if (_activator == null)
            Debug.LogError("Activator не назначен!", this);
        if (_colorPrecision == null)
            Debug.LogError("ColorPrecision не назначен!", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ColorableObject colorableObject) == false) return;

        if (_colorPrecision == null)
        {
            Debug.LogError("ColorPrecision not initialized!", this);
            return;
        }

        if (_renderer == null || _renderer.material == null)
        {
            Debug.LogError("Renderer or material missing!", this);
            return;
        }

        Color otherColor = colorableObject.GetColor();
        Color myColor = _renderer.material.color;

        if (otherColor == Color.white) return;

        if (_colorPrecision.Match(myColor, otherColor) == false) return;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        if (colorableObject is Block block)
        {
            _coroutine = StartCoroutine(WaitForComparison(block, otherColor));
            IsTouch?.Invoke(block);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ColorableObject _) == false)
            return;

        TouchEnded?.Invoke(other);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator WaitForComparison(Block block, Color color)
    {
        yield return _waitForSeconds;

        if (block != null)
        {
            if (effectsHandler != null)
                effectsHandler.Stop();
            
            block.Destroy(_indicator.transform, _inkSpawner.transform);
        }

        StartCoroutine(WaitSpawn(color));
        _coroutine = null;
    }

    private IEnumerator WaitSpawn(Color color)
    {
        yield return new WaitForSeconds(2);

        if (_activator != null)
        {
            _activator.EnqueueFragments(color);
        }
    }
}