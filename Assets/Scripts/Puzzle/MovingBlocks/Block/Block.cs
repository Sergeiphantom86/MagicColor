using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PathMover), typeof(ITouchDragInput))]
[RequireComponent(typeof(Collider), typeof(Voiceover))]
public class Block : ColorableObject, IDestroyable
{
    [Header("Grid")]
    [SerializeField] private Vector2Int _sizeInCells;

    [Header("Effects")]
    [SerializeField] private Rotator _rotation;
    [SerializeField] private Effecter _impactPool;
    [SerializeField] private Effecter _destructionPool;
    [SerializeField] private Effecter _smockPool;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _dragging;
    [SerializeField] private AudioClip _taking;

    private float _delayDisablingRender;
    private Vector2Int _gridPosition;
    private Collider _collider;
    private Voiceover _voiceover;
    private PathMover _pathMover;
    private InkSpawner _inkSpawner;
    private ITouchDragInput _touchDragInput;
    private WaitForSeconds _waitForSeconds;
    private CollisionHandler _collisionHandler;
    private GridDragMovement _gridDragMovement;
    private Magnifier _magnifier;
    private bool _isDestroyed;

    public bool IsDestroyed => _isDestroyed;

    public Vector2Int SizeInCells => _sizeInCells;
    public Vector2Int GridPosition => _gridPosition;

    public event Action<Block> OnDestroyed;
    public event Action BlockSpawned;

    private void Awake()
    {
        _delayDisablingRender = 1.3f;
        _waitForSeconds = new WaitForSeconds(_delayDisablingRender);
        _collider = GetComponent<Collider>();
        _voiceover = GetComponent<Voiceover>();
        _pathMover = GetComponent<PathMover>();
        _touchDragInput = GetComponent<ITouchDragInput>();
        _inkSpawner = GetComponentInChildren<InkSpawner>();
        _collisionHandler = GetComponent<CollisionHandler>();
        _gridDragMovement = GetComponent<GridDragMovement>();
        _magnifier = GetComponent<Magnifier>();
        _collider.enabled = false;
    }

    private void OnEnable()
    {
        InitializeComponents();
        _collisionHandler.OnEnter += PlaySound;
        _gridDragMovement.Moved += ShowEffectMovement;
        _magnifier.OnDropped += PlayFallingSound;
        _magnifier.OnRaised += PlayFallingSound;
    }

    private void OnDisable()
    {
        _collisionHandler.OnEnter -= PlaySound;
        _gridDragMovement.Moved -= ShowEffectMovement;
        _magnifier.OnDropped -= PlayFallingSound;
        _magnifier.OnRaised -= PlayFallingSound;
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public void Destroy(Vector3 waypoint, Vector3 endPoint)
    {
        if (_isDestroyed)
            return;

        _collider.enabled = false;
        _isDestroyed = true;

        _impactPool.CraeteParticles(transform.position, Quaternion.identity, 30);
        _impactPool.CreateEffect();

        _touchDragInput.ThrowOff();
        AssignOriginal();

        _voiceover.Play(_audioClip);

        _pathMover.Move(waypoint, endPoint, ExecuteDestruction);
    }

    public void Subscribe()
    {
        BlockSpawned?.Invoke();
    }

    public void TurnOnCollider()
    {
        _collider.enabled = true;
    }

    public void ResetState()
    {
        _gridPosition = default;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private void ExecuteDestruction()
    {
        SetRender();

        OnDestroyed?.Invoke(this);
        
        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ClearBlock(this);
        }

        _destructionPool.CraeteParticles(transform.position, Quaternion.identity, 1);

        if (_inkSpawner == null)
        {
            Debug.LogError("InkSpawner == null");
            return;
        }

        _inkSpawner.ActivateInkDrops(GetColor(), transform.lossyScale.x);

        StartCoroutine(WaitBeforeDisablingVisualization());
    }

    private IEnumerator WaitBeforeDisablingVisualization()
    {
        yield return _waitForSeconds;

        TurnOffRenderer();
    }

    private void PlaySound(Collider collider)
    {
        if (collider.TryGetComponent(out WallsContainer wallsContainer))
        {
            _voiceover.Play(_audioClip);
        }
    }

    private void ShowEffectMovement()
    {
        _voiceover.Play(_dragging);
        _smockPool.CraeteParticles(transform.position, Quaternion.identity, 0.3f);
    }

    private void PlayFallingSound()
    {
        _voiceover.Play(_taking);
    }
}