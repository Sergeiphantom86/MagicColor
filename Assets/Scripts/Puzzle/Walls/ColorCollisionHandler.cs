using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Collider), typeof(Rigidbody))]
public class ColorCollisionHandler : MonoBehaviour
{
    [SerializeField] private EffectsHandler effectsHandler;

    private float _delay;
    private Renderer _renderer;
    private Activator _activator;
    private Coroutine _coroutine;
    private WaitForSeconds _waitForSeconds;
    private IColorPrecision _colorPrecision;
    private CubeActivator _cubeActivator;

    public event Action<Block> IsTouch;
    public event Action<Collider> TouchEnded;

    private void Awake()
    {
        _delay = 0.1f;
        _renderer = GetComponent<Renderer>();
        _cubeActivator = GetComponentInChildren<CubeActivator>();
        _waitForSeconds = new WaitForSeconds(_delay);
    }

    public void Initialize(IColorPrecision colorPrecision, Activator activator)
    {
        _colorPrecision = colorPrecision;
        _activator = activator;

        if (_activator == null)
            throw new ArgumentNullException(nameof(_activator), "Activator не назначен!"); 
        if (_colorPrecision == null)
            throw new ArgumentNullException(nameof(_colorPrecision), "Activator не назначен!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ColorableObject colorableObject) == false)
            return;

        Color otherColor = colorableObject.GetColor();
        Color myColor = _renderer.material.color;

        if (otherColor == Color.white)
            return;

        if (_colorPrecision.Match(myColor, otherColor) == false)
            return;

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
            effectsHandler.Stop();
            _cubeActivator.TurnOff(color);
            block.Destroy();
        }

        StartCoroutine(WaitSpawn(color));
        _coroutine = null;
    }

    private IEnumerator WaitSpawn(Color color)
    {
        yield return new WaitForSeconds(2);

        _activator?.EnqueueFragments(color);
    }
}