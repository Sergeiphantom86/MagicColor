using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PathMover), typeof(ITouchDragInput))]
[RequireComponent(typeof(Collider), typeof(Voiceover))]
public class Block : ColorableObject, IDestroyable, IGridOccupant
{
    [Header("Grid")]
    [SerializeField] private Vector2Int _sizeInCells;

    private Effecter _effectImpact;
    private Effecter _effectDestruct;
    private Effecter _effectSmock;
    private AudioClip _soundDestruction;
    private AudioClip _soundDragg;
    private AudioClip _soundRaise;
    private Collider _collider;
    private Voiceover _voiceover;
    private Magnifier _magnifier;
    private PathMover _pathMover;
    private InkSpawner _inkSpawner;
    private WaitForSeconds _waitForSeconds;
    private GridDragMovement _gridDragMovement;
    private Vector2Int _gridPosition;
    private ITouchDragInput _touchDragInput;
    private float _delayDisablingRender;
    private bool _isDestroyed;

    public bool IsDestroyed => _isDestroyed;

    public Vector2Int SizeInCells => _sizeInCells;
    public Vector2Int GridPosition => _gridPosition;

    public GameObject GameObject => gameObject;

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
        _gridDragMovement = GetComponent<GridDragMovement>();
        _magnifier = GetComponent<Magnifier>();
        _collider.enabled = false;
    }

    private void OnEnable()
    {
        InitializeComponents();
        _gridDragMovement.Moved += ShowEffectMovement;
        _magnifier.OnDropped += PlayFallingSound;
        _magnifier.OnRaised += PlayFallingSound;
    }

    private void OnDisable()
    {
        _gridDragMovement.Moved -= ShowEffectMovement;
        _magnifier.OnDropped -= PlayFallingSound;
        _magnifier.OnRaised -= PlayFallingSound;
    }

    public void Initializat(Effecter effectImpact, Effecter effectSmock, Effecter effectDestruct, AudioClip soundDestruction, AudioClip soundDragg, AudioClip soundRaise)
    {
        _effectImpact = effectImpact;
        _effectDestruct = effectDestruct;
        _effectSmock = effectSmock;
        _soundDestruction = soundDestruction;
        _soundDragg = soundDragg;
        _soundRaise = soundRaise;
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

        _effectImpact.CraeteParticles(transform.position, Quaternion.identity, 1);
        _effectImpact.Create();

        _touchDragInput.ThrowOff();
        AssignOriginal();

        _voiceover.PlayOneShot(_soundDestruction);

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
        OnDestroyed?.Invoke(this);

        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ClearCell(this);
        }

        _effectDestruct.CraeteParticles(transform.position, Quaternion.identity, 1);

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

    private void ShowEffectMovement()
    {
        _voiceover.PlayOneShot(_soundDragg);
        _effectSmock.CraeteParticles(transform.position, Quaternion.identity, 0.3f);
    }

    private void PlayFallingSound()
    {
        _voiceover.PlayOneShot(_soundRaise);
    }
}