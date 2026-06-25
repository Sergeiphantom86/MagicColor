using System;
using System.Collections;
using DG.Tweening;
using Menu;
using PuzzleEditor.MovingBlocks.GridEditor;
using PuzzleEditor.PoolEffects;
using PuzzleEditor.SoundEditor;
using PuzzleEditor.Spawners;
using UnityEngine;

namespace PuzzleEditor.MovingBlocks.BlockEditor
{
    [RequireComponent(typeof(GridDragMovement))]
    [RequireComponent(typeof(Collider), typeof(Voiceover), typeof(Scaler))]
    [RequireComponent(typeof(PathMover), typeof(ITouchDragInput), typeof(Magnifier))]
    public class Block : ColorableObject, IDestroyable, IGridOccupant
    {
        [Header("Grid")]
        [SerializeField]
        private Vector2Int _sizeInCells;

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
        private Vector2Int _gridPosition;
        private ITouchDragInput _touchDragInput;
        private Outline _outline;

        public event Action<Block> OnDestroyed;

        public event Action BlockSpawned;

        public Vector2Int SizeInCells => _sizeInCells;

        public Vector2Int GridPosition => _gridPosition;

        public GameObject GameObject => gameObject;

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

            _collider.enabled = false;
            _outline.OutlineColor = Color.yellow;
            _outline.enabled = false;
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

        public void Initialize(
            Effecter effectImpact,
            Effecter effectSmock,
            Effecter effectDestruct,
            AudioClip soundDestruction,
            AudioClip soundDragg,
            AudioClip soundRaise,
            AudioClip matchSound
        )
        {
            _soundDragg = soundDragg;
            _soundRaise = soundRaise;
            _matchSound = matchSound;
            _effectSmock = effectSmock;
            _effectImpact = effectImpact;
            _effectDestruct = effectDestruct;
            _soundDestruction = soundDestruction;
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

            _effectImpact.CraeteParticles(
                transform.position,
                Quaternion.identity,
                _scaleImpactEffect
            );

            _touchDragInput.ThrowOff();

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
            OnDestroyed?.Invoke(this);

            if (GridSystem.Instance != null)
            {
                GridSystem.Instance.ClearCell(this);
            }

            _effectDestruct.CraeteParticles(
                transform.position,
                Quaternion.identity,
                _scaleDestructEffect
            );

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

            _inkSpawner.ActivateInkDrops(GetColor(), _duration);
        }

        private void ReduceSize()
        {
            _scaler.GetTwinResiz(Vector3.zero, _duration).SetEase(Ease.InOutElastic);
        }

        private void ShowEffectMovement()
        {
            _voiceover.PlayOneShot(_soundDragg);
            _effectSmock.CraeteParticles(
                transform.position,
                Quaternion.identity,
                _scaleSmockEffect
            );
        }

        private void PlayFallingSound()
        {
            _voiceover.PlayOneShot(_soundRaise);
        }
    }
}