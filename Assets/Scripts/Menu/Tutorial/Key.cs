using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InputHandler), typeof(ICollisionHandler), typeof(Voiceover))]
public class Key : Currency
{
    [SerializeField] private Point _startPoint;
    [SerializeField] private Point _endPoint;
    [SerializeField] private AudioClip _flight;
    [SerializeField] private AudioClip _hiding;
    [SerializeField] private AudioClip _appearance;

    private float _zoomIn;
    private float _zoomOut;
    private bool _isDragging;
    private string _quantity;
    private float _movementDuration;
    private float _delayBetweenMovements;
    private Voiceover _voiceover;
    private Vector3 _rotationAngles;
    private Sequence _movementSequence;
    private InputHandler _inputHandler;
    private ICollisionHandler _collisionHandler;
    private SpriteRenderer _spriteRenderer;

    public event Action OnShift;
    public event Action OnSelected;

    private void Awake()
    {
        _quantity = "1";
        _zoomIn = 4;
        _zoomOut = 1;
        _isDragging = true;
        _movementDuration = 1;
        _delayBetweenMovements = 0.5f;
        _rotationAngles = new Vector3(-25, 0, 0);
        _voiceover = GetComponent<Voiceover>();
        _inputHandler = GetComponent<InputHandler>();
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

    private void Play(Vector2 vector)
    {
        if (_isDragging) return;

        _isDragging = true;

        _movementSequence.Play();
        
        StartCoroutine(WaitAudioPlayback(_flight));

        OnSelected?.Invoke();

    }

    private IEnumerator WaitAudioPlayback(AudioClip clip)
    {
        _voiceover.Play(clip);
        yield return new WaitForSeconds(clip.length);
        _voiceover.Play(clip);
    }

    private void Hide(Collider collider)
    {
        if (collider.TryGetComponent(out Block _) == false) return;

        _spriteRenderer.enabled = false;
        _voiceover.Play(_hiding);
    }

    private void Show(Collider collider)
    {
        if (collider.TryGetComponent(out Block _) == false) return;

        _isDragging = false;
        _spriteRenderer.enabled = true;

        _voiceover.Play(_appearance);

        OnShift?.Invoke();
    }

    private void CreateAnimationSequences()
    {
        _movementSequence = DOTween.Sequence();

        _spriteRenderer.rendererPriority = 1;

        _movementSequence
            .Append(BuildMove(
                _startPoint.transform.position,
                _movementDuration,
                _rotationAngles,
                transform.localScale.x * _zoomIn,
                Ease.OutBounce
            ))
            .AppendInterval(_delayBetweenMovements)
            .Append(BuildMove(
                _endPoint.transform.position,
                _movementDuration * 4,
                -_rotationAngles,
                transform.localScale.x * _zoomOut,
                Ease.InOutBack
            ));

        _movementSequence.Pause();
    }

    private Sequence BuildMove(Vector3 position, float duration, Vector3 rotationAngles, float scaleMultiplier, Ease ease)
    {
        return DOTween.Sequence()
            .Append(transform.DOMove(position, duration))
            .Join(transform.DORotate(rotationAngles, duration))
            .Join(transform.DOScale(scaleMultiplier, duration))
             .SetEase(ease);
    }

    private void OnDestroy()
    {
        _movementSequence?.Kill();
    }
}