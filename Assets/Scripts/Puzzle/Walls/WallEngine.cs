using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ColorCollisionHandler))]
public class WallEngine : MonoBehaviour
{
    private bool _isMoving;
    private float _moveDuration;
    private float _pushDistance;
    private float _distanceBlock;
    private Vector3 _startPosition;
    private InputHandler _handler;
    private ColorCollisionHandler _colorCollisionHandler;
    private Wall _wall;
    private Bag _bag;

    private void Awake()
    {
        _moveDuration = 0.3f;
        _pushDistance = 1f;
        _distanceBlock = 0.3f;
        _handler = GetComponent<InputHandler>();
        _wall = GetComponent<Wall>();
        _colorCollisionHandler = GetComponent<ColorCollisionHandler>();
    }

    private void OnEnable()
    {
        if (_handler == null && _colorCollisionHandler == null) return;

        _colorCollisionHandler.IsTouch += OnBlockTouch;
        _handler.OnSelected += TryRemoveLock;
    }

    private void OnDisable()
    {
        if (_handler == null && _colorCollisionHandler == null) return;

        _colorCollisionHandler.IsTouch -= OnBlockTouch;
        _handler.OnSelected -= TryRemoveLock;
    }

    public void Initialize(IColorPrecision colorPrecision, Activator activator, Bag bag)
    {
        _colorCollisionHandler.Initialize(colorPrecision, activator);

        _bag = bag;
    }

    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    private void TryRemoveLock(Vector2 position)
    {
        if (_bag.SpendFunds(1))
        {
            _colorCollisionHandler.UnblockWall();

            return;
        }
        
        StartMovement(GetDistans());
    }

    private void OnBlockTouch(Block block)
    {
        StartMovement(GetDistans());
    }

    private float GetDistans()
    {
        if (_wall.IsBlocked)
        {
            return _distanceBlock;
        }

        return _pushDistance;
    }

    private void StartMovement(float pushDistance)
    {
        if (_isMoving == false)
        {
            _isMoving = true;

            SetStartPosition();

            GetSequence(_startPosition + Vector3.down * pushDistance, _moveDuration)
               .OnComplete(() =>
               {
                   ReturnToStart();
               });
        }
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