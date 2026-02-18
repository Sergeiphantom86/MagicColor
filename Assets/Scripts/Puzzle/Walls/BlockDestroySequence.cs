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

    public event Action<Block> IsTouched;

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

        colorable.SetRenderQueue();

        IsTouched?.Invoke(block);

        StartCoroutine(Run(colorable, block, color));
    }

    private IEnumerator Run(IColorable colorable, Block block, Color color)
    {
        yield return _waitShutdown;

        colorable.ReturnRenderQueue();

        block.Destroy(_pointer.MiddlePoint, _pointer.EndPoint);

        yield return _waitActivat;

        _activator.EnqueueFragments(color);
    }
}