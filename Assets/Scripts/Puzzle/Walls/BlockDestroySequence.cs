using System;
using System.Collections;
using UnityEngine;

public class BlockDestroySequence : MonoBehaviour, IBlockDestroySequence
{
    private Activator _activator;
    private IPointer _pointer;
    private WaitForSeconds _waitShutdown;
    private WaitForSeconds _waitActivat;
    private float _delayShutdown;
    private float _delayActivat;
    private bool _isCollaps;

    public event Action IsTouched;

    public void Initialize(Activator activator)
    {
        _activator = activator;
        _pointer = GetComponent<IPointer>();
    }

    private void Awake()
    {
        _delayShutdown = 0.1f;
        _delayActivat = 2;
        _waitShutdown = new WaitForSeconds(_delayShutdown);
        _waitActivat = new WaitForSeconds(_delayActivat);
    }

    public void WaitStart(IColorable colorable, Color color)
    {
        if (colorable is not Block block)
            return;

        if (_isCollaps == false)
        {
            _isCollaps = true;

            IsTouched?.Invoke();

            StartCoroutine(Run(block, color));
        }
    }

    private IEnumerator Run(Block block, Color color)
    {
        yield return _waitShutdown;
        yield return _waitShutdown;
        yield return _waitShutdown;

        block.PlayMatchSound();
        yield return _waitShutdown;

        block.Destroy(_pointer.MiddlePoint, _pointer.EndPoint);

        yield return _waitActivat;

        _activator.EnqueueFragments(color);

        _isCollaps = false;
    }
}