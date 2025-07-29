using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ColorCollisionHandler), typeof(Rigidbody))]
public class WallEngine : MonoBehaviour
{
    private bool _isMoving;
    private float _moveDuration;
    private float _pushDistance;
    private Vector3 _startPosition;
    private ColorCollisionHandler _colorCollisionHandler;

    private void Awake()
    {
        _moveDuration = 0.3f;
        _pushDistance = 1f;

        _colorCollisionHandler = GetComponent<ColorCollisionHandler>();
    }

    private void Start()
    {
        SetStartPosition();
    }

    private void OnEnable()
    {
        _colorCollisionHandler.IsTouch += OnBlockTouch;
    }

    private void OnDisable()
    {
        _colorCollisionHandler.IsTouch -= OnBlockTouch;
    }

    public void Initialize(IColorPrecision colorPrecision, Activator activator)
    {
        _colorCollisionHandler.Initialize(colorPrecision, activator);
    }

    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    private void OnBlockTouch(Block block)
    {
        if (_isMoving || block == null) return;
        StartMovement();
    }

    private void StartMovement()
    {
        if (_isMoving) return;
        
        _isMoving = true;

        GetSequence(_startPosition + Vector3.down * _pushDistance, _moveDuration)
           .OnComplete(() =>
           {
               ReturnToStart();
           });
    }

    private void ReturnToStart()
    {
        GetSequence(_startPosition, _moveDuration)
          .OnComplete(() =>
          {
              _isMoving = false;
          });
    }

    private Tweener GetSequence(Vector3 position, float duration)
    {
        return transform.DOMove(position, duration)
           .SetEase(Ease.InOutQuad);
    }
}