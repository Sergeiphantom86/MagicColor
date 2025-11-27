using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Indicator), typeof(IColorable))]
[RequireComponent(typeof(Wall))]
public class ColorCollisionHandler : MonoBehaviour
{
    [SerializeField] private EffectsHandler effectsHandler;

    private float _delay;
    private Wall _wall;
    private Lock  _lock;
    private Point _point;
    private Renderer _renderer;
    private Indicator _indicator;
    private Activator _activator;
    private Coroutine _coroutine;
    private IColorable _colorable;
    private IColorPrecision _colorPrecision;
    private WaitForSeconds _waitForSeconds;

    public event Action<Block> IsTouch;
    public event Action<Collider> TouchEnded;

    private void Awake()
    {
        _delay = 0.1f;
        _wall = GetComponent<Wall>();
        _renderer = GetComponent<Renderer>();
        _colorable = GetComponent<IColorable>();
        _indicator = GetComponent<Indicator>();
        _point = GetComponentInChildren<Point>();
        _waitForSeconds = new WaitForSeconds(_delay);


        if (_renderer == null || _renderer.material == null)
        {
            Debug.LogError("Renderer не назначен!", this);
            return;
        }

        if (TryGetComponent(out IColorable colorable))
        {
            _colorable = colorable;
        }

        if (_indicator == null)
        {
            Debug.LogError("Indicator не назначен!", this);
            return;
        }

        if (_point == null)
        {
            Debug.LogError("Point не назначен!", this);
            return;
        }
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

    public void UnblockWall()
    {
        _lock.Unblock();
    }

    public void TriggerContactEvent(Block block)
    {
        IsTouch?.Invoke(block);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Lock @lock))
        {
            _lock = @lock;
        }

        if (other.TryGetComponent(out ColorableObject colorableObject) == false) return;

        _colorable.AssignOriginal();

        Color otherColor = colorableObject.GetColor();

        if (otherColor == Color.white) return;

        if (_colorPrecision.Match(_renderer.material.color, otherColor) == false) return;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        if (colorableObject is Block block && _wall.IsBlocked == false)
        {
            _coroutine = StartCoroutine(WaitForComparison(block, otherColor));

            IsTouch?.Invoke(block);
        }
        else if(_lock != null)
        {
            _lock.ShakeUp();

            IsTouch?.Invoke(null);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Lock _))
        {
            _wall.Unblock();
        }

        if (other.TryGetComponent(out ColorableObject _) == false) return;

        TouchEnded?.Invoke(other);

        _colorable.Disable();

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

            _colorable.Disable();

            block.Destroy(_indicator.transform, _point.transform);
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