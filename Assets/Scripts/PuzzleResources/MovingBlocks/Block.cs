using System;
using System.Collections;
using DG.Tweening;
using Menu;
using PuzzleResources.MovingBlocks.GridLogic;
using PuzzleResources.PoolEffects;
using PuzzleResources.Audio;
using PuzzleResources.Spawners;
using UnityEngine;

namespace PuzzleResources.MovingBlocks
{
    [RequireComponent(typeof(GridDragMovement))]
    [RequireComponent(typeof(Collider), typeof(Voiceover), typeof(Scaler))]
    [RequireComponent(typeof(PathMover), typeof(ITouchDragInput), typeof(Magnifier))]
    public class Block : ColorableObject, IGridOccupant
    {
        [Header("Grid")]
        [SerializeField] private Vector2Int _sizeInCells;

        private float _duration;
        private float _scaleSmockEffect;
        private float _scaleImpactEffect;
        private float _scaleDestructEffect;

        private Scaler _scaler;
        private Collider _collider;
        private PathMover _pathMover;
        private Voiceover _voiceover;
        private Magnifier _magnifier;
        private Effecter _effectSmock;
        private Effecter _effectImpact;
        private Effecter _effectDestruct;
        private AudioClip _soundDragg;
        private AudioClip _soundRaise;
        private AudioClip _soundDestruction;
        private AudioClip _matchSound;
        private InkSpawner _inkSpawner;
        private GridDragMovement _gridDragMovement;
        private WaitForSeconds _waitForDuration;
        private Vector2Int _gridPosition;
        private ITouchDragInput _touchDragInput;
        private Outline _outline;
        private GridSystem _gridSystem;

        public event Action<Block> Destroyed;

        public event Action BlockSpawned;

        public Vector2Int SizeInCells => _sizeInCells;

        public Vector2Int GridPosition => _gridPosition;

        public GameObject GameObject => gameObject;

        public GridSystem GridSystem => _gridSystem;

        private void Awake()
        {
            _duration = 0.5f;
            _scaleImpactEffect = 1;
            _scaleSmockEffect = 0.3f;
            _scaleDestructEffect = 1;

            _scaler = GetComponent<Scaler>();
            _outline = GetComponent<Outline>();
            _collider = GetComponent<Collider>();
            _voiceover = GetComponent<Voiceover>();
            _pathMover = GetComponent<PathMover>();
            _magnifier = GetComponent<Magnifier>();
            _touchDragInput = GetComponent<ITouchDragInput>();
            _gridDragMovement = GetComponent<GridDragMovement>();
            _inkSpawner = GetComponentInChildren<InkSpawner>();
            _waitForDuration = new WaitForSeconds(_duration);

            _collider.enabled = false;
            _outline.OutlineColor = Color.yellow;
            _outline.enabled = false;
        }

        private void OnEnable()
        {
            InitializeComponents();

            _gridDragMovement.Moved += OnShowEffectMovement;
            _magnifier.Dropped += OnPlayFallingSound;
            _magnifier.Raised += OnPlayFallingSound;
        }

        private void OnDisable()
        {
            _gridDragMovement.Moved -= OnShowEffectMovement;
            _magnifier.Dropped -= OnPlayFallingSound;
            _magnifier.Raised -= OnPlayFallingSound;
        }

        public void Initialize(
            Effecter effectImpact,
            Effecter effectSmock,
            Effecter effectDestruct,
            AudioClip soundDestruction,
            AudioClip soundDragg,
            AudioClip soundRaise,
            AudioClip matchSound,
            GridSystem gridSystem)
        {
            _soundDragg = soundDragg;
            _soundRaise = soundRaise;
            _matchSound = matchSound;
            _effectSmock = effectSmock;
            _effectImpact = effectImpact;
            _effectDestruct = effectDestruct;
            _soundDestruction = soundDestruction;
            _gridSystem = gridSystem;

            _gridDragMovement.SetGridSystem(gridSystem);
        }

        public void SetOutlineColor()
        {
            _outline.enabled = true;
        }

        public void SetGridPosition(Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public void PlayMatchSound()
        {
            _voiceover.PlayOneShot(_matchSound);
        }

        public void Destroy(Vector3 waypoint, Vector3 endPoint)
        {
            _collider.enabled = false;
            _outline.enabled = false;

            SetRenderQueue();

            _effectImpact.CraeteParticles(transform.position, Quaternion.identity, _scaleImpactEffect);

            _touchDragInput.OnThrowOff();

            AssignOriginal();

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

        private void ExecuteDestruction()
        {
            Destroyed?.Invoke(this);

            if (_gridSystem != null)
            {
                _gridSystem.ClearCell(this);
            }

            _effectDestruct.CraeteParticles(transform.position, Quaternion.identity, _scaleDestructEffect);

            if (_inkSpawner == null)
            {
                Debug.LogError("InkSpawner == null");
                return;
            }

            StartCoroutine(WaitBeforeDisablingVisualization());
        }

        private IEnumerator WaitBeforeDisablingVisualization()
        {
            ReduceSize();

            _voiceover.PlayOneShot(_soundDestruction);

            Tween fadeTween = TurnOffRenderer();

            if (fadeTween != null)
                yield return fadeTween.WaitForCompletion();

            _inkSpawner.ActivateInkDrops(GetColor(), _waitForDuration);
        }

        private void ReduceSize()
        {
            _scaler.GetTwinResiz(Vector3.zero, _duration).SetEase(Ease.InOutElastic);
        }

        private void OnShowEffectMovement()
        {
            _voiceover.PlayOneShot(_soundDragg);
            _effectSmock.CraeteParticles(transform.position, Quaternion.identity, _scaleSmockEffect);
        }

        private void OnPlayFallingSound()
        {
            _voiceover.PlayOneShot(_soundRaise);
        }
    }
}