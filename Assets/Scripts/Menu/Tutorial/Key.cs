using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(InputHandler), typeof(CollisionHandler), typeof(Voiceover))]
public class Key : Currency
{
    [SerializeField] private Point _startPoint;
    [SerializeField] private Point _endPoint;
    [SerializeField] private AudioClip _flight;
    [SerializeField] private AudioClip _hiding;
    [SerializeField] private AudioClip _appearance;

    private string _quantity;
    private float _zoomIn;
    private float _zoomOut;
    private bool _isDragging;
    private float _movementDuration;
    private float _delayBetweenMovements;
    private Vector3 _rotationAngles;
    private ParticleSystem _shine;
    private Sequence _movementSequence;
    private InputHandler _inputHandler;
    private CollisionHandler _collisionHandler;
    private Voiceover _voiceover;

    public event Action OnShift;
    public event Action OnSelected;

    private void Awake()
    {
        _quantity = "1";
        _zoomIn = 0.5f;
        _zoomOut = 0.1f;
        _isDragging = true;
        _movementDuration = 1f;
        _delayBetweenMovements = 0.5f;
        _rotationAngles = new Vector3(30, 0, 40);
        _voiceover = GetComponent<Voiceover>();
        _inputHandler = GetComponent<InputHandler>();
        _collisionHandler = GetComponent<CollisionHandler>();
        _shine = GetComponentInChildren<ParticleSystem>();

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

        if (_shine == null)
        {
            Debug.LogError("ParticleSystem == null");
        }

        _shine.Stop();
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
        _voiceover.Play(_flight);
        OnSelected?.Invoke();

    }

    private void Hide(Collider collider)
    {
        if (collider.TryGetComponent(out Block _) == false) return;

        Icon.enabled = false;
        _voiceover.Play(_hiding);
        _shine.Stop();

    }

    private void Show(Collider collider)
    {
        if (collider.TryGetComponent(out Block _) == false) return;

        _isDragging = false;

        Icon.enabled = true;

        _shine.Play();
        _voiceover.Play(_appearance);
        OnShift?.Invoke();

    }

    private void CreateAnimationSequences()
    {
        _movementSequence = DOTween.Sequence();

        CreateAnimationSequence(_startPoint.transform.position, _movementDuration, _rotationAngles, _zoomIn, Ease.OutBounce);

        _movementSequence.AppendInterval(_delayBetweenMovements);

        CreateAnimationSequence(_endPoint.transform.position, _movementDuration, -_rotationAngles, _zoomOut, Ease.InOutBack);

        _movementSequence.Pause();
    }

    private void CreateAnimationSequence(Vector3 position, float duration, Vector3 rotationAngles, float scaleMultiplier, Ease ease)
    {
        _movementSequence.Append(transform.DOMove(position, duration))
            .Join(transform.DORotate(rotationAngles, duration))
            .Join(transform.DOScale(scaleMultiplier, duration))
            .SetEase(ease).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void OnDestroy()
    {
        _movementSequence?.Kill();
    }
}