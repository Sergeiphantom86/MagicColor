using System;
using System.Collections;
using DG.Tweening;
using PuzzleEditor;
using PuzzleEditor.MovingBlocks;
using PuzzleEditor.MovingBlocks.BlockEditor;
using PuzzleEditor.RouletteEditor;
using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace Menu.TutorialEditor
{
    [RequireComponent(typeof(IInputHandler), typeof(ICollisionHandler), typeof(Voiceover))]
    public class Key : Currency, IActivatable
    {
        [SerializeField]
        private PuzzleEditor.Walls.Point _startPoint;

        [SerializeField]
        private PuzzleEditor.Walls.Point _endPoint;

        [SerializeField]
        private AudioClip _flight;

        [SerializeField]
        private AudioClip _hiding;

        [SerializeField]
        private AudioClip _appearance;

        private float _zoomIn;
        private float _zoomOut;
        private bool _isDragging;
        private string _quantity;
        private float _movementDuration;
        private Voiceover _voiceover;
        private Sequence _movementSequence;
        private IInputHandler _inputHandler;
        private ICollisionHandler _collisionHandler;
        private SpriteRenderer _spriteRenderer;

        public event Action OnShift;

        public event Action OnSelected;

        private void Awake()
        {
            _quantity = "1";
            _zoomIn = 2;
            _zoomOut = 1;
            _isDragging = true;
            _movementDuration = 0.5f;
            _voiceover = GetComponent<Voiceover>();
            _inputHandler = GetComponent<IInputHandler>();
            _collisionHandler = GetComponent<ICollisionHandler>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_voiceover == null)
            {
                Debug.LogError("Voiceover == null");
            }

            if (_inputHandler == null)
            {
                Debug.LogError("InputHandler == null");
            }

            if (_collisionHandler == null)
            {
                Debug.LogError("CollisionHandler == null");
            }

            if (_spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer == null");
            }

            SetValue(_quantity);
            TurnOffDisplay();
        }

        private void Start()
        {
            CreateAnimationSequences();
        }

        private void OnEnable()
        {
            _inputHandler.OnSelected += Play;
            _collisionHandler.OnExit += Show;
            _collisionHandler.OnEnter += Hide;
        }

        private void OnDisable()
        {
            _inputHandler.OnSelected -= Play;
            _collisionHandler.OnExit -= Show;
            _collisionHandler.OnEnter -= Hide;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void Play(Vector2 vector)
        {
            if (_isDragging)
                return;

            _isDragging = true;

            _movementSequence.Play();

            StartCoroutine(WaitAudioPlayback(_flight));

            OnSelected?.Invoke();
        }

        private IEnumerator WaitAudioPlayback(AudioClip clip)
        {
            _voiceover.PlayOneShot(clip);

            yield return new WaitForSeconds(clip.length);

            _voiceover.PlayOneShot(clip);
        }

        private void Hide(Collider collider)
        {
            if (collider.TryGetComponent(out Block _) == false)
                return;

            TurnOffDisplay();
            _voiceover.PlayOneShot(_hiding);
        }

        private void TurnOffDisplay()
        {
            _spriteRenderer.enabled = false;
        }

        private void TurnOnDisplay()
        {
            _spriteRenderer.enabled = true;
        }

        private void Show(Collider collider)
        {
            if (collider.TryGetComponent(out Block block) == false)
                return;

            if (block.TryGetComponent(out Outline outline))
            {
                outline.enabled = false;
            }

            _isDragging = false;

            TurnOnDisplay();

            _voiceover.PlayOneShot(_appearance);

            OnShift?.Invoke();
        }

        private void CreateAnimationSequences()
        {
            _movementSequence = DOTween.Sequence();

            _spriteRenderer.rendererPriority = 1;

            _movementSequence
                .Append(
                    BuildMove(
                        _startPoint.transform.position,
                        _movementDuration,
                        transform.localScale.x * _zoomIn,
                        Ease.OutBounce
                    )
                )
                .Append(
                    BuildMove(
                        _endPoint.transform.position,
                        _movementDuration * 4,
                        transform.localScale.x * _zoomOut,
                        Ease.InOutBack
                    )
                );

            _movementSequence.Pause();
        }

        private Sequence BuildMove(
            Vector3 position,
            float duration,
            float scaleMultiplier,
            Ease ease
        )
        {
            return DOTween
                .Sequence()
                .Append(transform.DOMove(position, duration))
                .Join(transform.DOScale(scaleMultiplier, duration))
                .SetEase(ease);
        }

        private void OnDestroy()
        {
            _movementSequence?.Kill();
        }
    }
}