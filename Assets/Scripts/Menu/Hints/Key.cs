using DG.Tweening;
using UnityEngine;

public class Key : Currency
{
    [SerializeField] private Point _startPoint;
    [SerializeField] private Point _endPoint;

    private string _value;
    private float _zoomIn;
    private float _zoomOut;
    private bool _isDragging;
    private Vector3 _rotationAngles;
    private float _movementDuration;
    private float _delayBetweenMovements;
    private ParticleSystem _shine;
    private Sequence _movementSequence;
    private InputHandler _inputHandler;
    private CollisionHandler _collisionHandler;

    private void Awake()
    {
        _value = "1";
        _zoomIn = 0.5f;
        _zoomOut = 0.1f;
        _movementDuration = 1f;
        _delayBetweenMovements = 0.5f;
        _rotationAngles = new Vector3(30, 0, 40);
        _isDragging = true;
        _inputHandler = GetComponent<InputHandler>();
        _collisionHandler = GetComponent<CollisionHandler>();
        _shine = GetComponentInChildren<ParticleSystem>();

        _shine.Stop();
        SetValue(_value);
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
        if (_isDragging == false)
        {
            CreateAnimationSequences();
        }
    }

    private void Hide(Collider collider)
    {
        if (collider.TryGetComponent(out Block blok))
        {
            Icon.enabled = false;
            _shine.Stop();
        }
    }

    private void Show(Collider collider)
    {
        if (collider.TryGetComponent(out Block blok))
        {
            Icon.enabled = true;
            _shine.Play();
            _isDragging = false;
        }
    }

    private void CreateAnimationSequences()
    {
        _movementSequence = DOTween.Sequence();
        _isDragging = true;
        CreateAnimationSequence(_startPoint.transform.position, _movementDuration, _rotationAngles, _zoomIn, Ease.OutBounce);

        _movementSequence.AppendInterval(_delayBetweenMovements);

        CreateAnimationSequence(_endPoint.transform.position, _movementDuration, -_rotationAngles, _zoomOut, Ease.InOutBack);
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