using CartoonFX;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PathMover), typeof(TouchDragInput), typeof(Renderer))]
[RequireComponent(typeof(Collider), typeof(Voiceover))]
public class Block : ColorableObject, IDestroyable
{
    [SerializeField] private Rotator _rotation;
    [SerializeField] private ParticleSystemPool _impactPool;
    [SerializeField] private ParticleSystemPool _destructionPool;

    private float _delay;
    private Vector2Int _gridPosition;
    private Renderer _renderer;
    private Collider _collider;
    private Voiceover _voiceover;
    private AudioClip _audioClip;
    private PathMover _pathMover;
    private InkSpawner _inkSpawner;
    private CFXR_Effect _cFXR_Effect;
    private TouchDragInput _touchDragInput;
    private WaitForSeconds _waitForSeconds;
    public event Action<Block> OnDestroyed;

    private void Awake()
    {
        _delay = 1.3f;
        _waitForSeconds = new WaitForSeconds(_delay);
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _voiceover = GetComponent<Voiceover>();
        _pathMover = GetComponent<PathMover>();
        _touchDragInput = GetComponent<TouchDragInput>();
        _inkSpawner = GetComponentInChildren<InkSpawner>();

        _collider.enabled = true;
    }

    public void  Initialize(AudioClip audioClip)
    {
        _audioClip = audioClip;

        InitializeComponents();
    }

    public void Destroy(Transform waypoint, Transform endPoint)
    {
        ParticleSystem impactEffect = GetEffect(_impactPool.Pool.Get());

        ApplyEffect(impactEffect);

        StartCoroutine(ReturnToPoolAfterDelay(impactEffect, _impactPool, impactEffect.main.duration));

        _collider.enabled = false;

        LetGo();

        AssignOriginal();

        _voiceover.Play(_audioClip);
        _pathMover.Move(waypoint, endPoint, ExecuteDestruction);
    }

    private void ApplyEffect(ParticleSystem impactEffect)
    {
        _cFXR_Effect = impactEffect.GetComponent<CFXR_Effect>();

        if (_cFXR_Effect != null)
        {
            _cFXR_Effect.ResetState();

            if (_cFXR_Effect.cameraShake != null)
            {
                _cFXR_Effect.cameraShake.FetchCameras();
                _cFXR_Effect.cameraShake.StartShake();
            }
        }
    }


    private void LetGo()
    {
        _touchDragInput.ThrowOff();
    }

    private void ExecuteDestruction()
    {
        OnDestroyed?.Invoke(this);

        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ClearCell(_gridPosition);
        }

        ParticleSystem destructionEffect = GetEffect(_destructionPool.Pool.Get());

        _inkSpawner.ActivateInkDrops(GetColor(), destructionEffect);

        StartCoroutine(ReturnToPoolAfterDelay(destructionEffect, _destructionPool, destructionEffect.main.duration));

        StartCoroutine(WaitBeforeDisablingVisualization());
    }

    private ParticleSystem GetEffect(ParticleSystem particleSystem)
    {
        particleSystem.transform.position = transform.position;
        particleSystem.gameObject.SetActive(true);
        particleSystem.Play();

        return particleSystem;
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        _gridPosition = gridPosition;
    }

    private IEnumerator WaitBeforeDisablingVisualization()
    {
        yield return _waitForSeconds;

        _renderer.enabled = false;
    }

    private IEnumerator ReturnToPoolAfterDelay(ParticleSystem effect, ParticleSystemPool pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Pool.Release(effect);
    }
}