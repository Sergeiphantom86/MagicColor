using System;
using System.Collections;
using UnityEngine;

public class BlockDestroySequence : MonoBehaviour, IBlockDestroySequence
{
    private EffectsHandler _effects;
    private Activator _activator;

    private WaitForSeconds _waitShutdown;
    private WaitForSeconds _waitActivat;
    private float _delayShutdown;
    private float _delayActivat;

    public event Action<Block> IsTouched;

    public void Initialize(EffectsHandler effects, Activator activator)
    {
        _effects = effects;
        _activator = activator;
    }

    private void Awake()
    {
        _delayShutdown = 0.1f;
        _delayActivat = 2;
        _waitShutdown = new WaitForSeconds(_delayShutdown);
        _waitActivat = new WaitForSeconds(_delayActivat);
    }

    public void WaitStart(Block block, Color color, Wall wall)
    {
        IsTouched?.Invoke(block);

        StartCoroutine(Run(block, color, wall));
    }

    private IEnumerator Run(Block block, Color color, Wall wall)
    {
        yield return _waitShutdown;

        _effects.Stop();
        block.Destroy(wall.MiddlePoint, wall.EndPoint);

        yield return _waitActivat;
        _activator.EnqueueFragments(color);
    }
}