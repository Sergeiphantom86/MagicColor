using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PathMover), typeof(TouchDragInput), typeof(Renderer))]
[RequireComponent(typeof(Collider), typeof(Voiceover))]
public class Block : ColorableObject, IDestroyable
{
    [SerializeField] private Rotator _rotation;

    private float _delay;
    private Vector2Int _gridPosition;
    private Renderer _renderer;
    private Collider _collider;
    private Voiceover _voiceover;
    private AudioClip _audioClip;
    private PathMover _pathMover;
    private InkSpawner _inkSpawner;
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
        _collider.enabled = false;
        LetGo();
        AssignOriginal();
        _voiceover.Play(_audioClip);
        _pathMover.Move(waypoint, endPoint, ExecuteDestruction);
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

        _inkSpawner.ActivateInkDrops(GetColor());

       StartCoroutine(WaitBeforeDisablingVisualization());
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
}