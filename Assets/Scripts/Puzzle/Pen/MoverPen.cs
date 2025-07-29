using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PenTiltController), typeof(PenScaleController))]
public class MoverPen : MonoBehaviour, IMover
{
    private const float ScreenCenterRatio = 0.5f;

    private Camera _mainCamera;
    private PenTiltController _tiltController;
    private PenScaleController _scaleController;
    private Vector3 _basePosition;
    private Vector3 _screenCenter;
    private float _xOffset;
    private float _targetAngle;
    private float _normalizedOffset;
    private bool _isProgrammaticMove;
    private Action _onMoveComplete;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _tiltController = GetComponent<PenTiltController>();
        _scaleController = GetComponent<PenScaleController>();

        _screenCenter = _mainCamera.ViewportToWorldPoint(
           new Vector3(ScreenCenterRatio, ScreenCenterRatio, _mainCamera.nearClipPlane));
    }

    private void Start() =>
        _tiltController.Initialize(transform.localEulerAngles.z);

    private void Update()
    {
        if (_isProgrammaticMove) return;

        if (HasPositionChanged())
        {
            UpdateTilt();
            UpdateBasePosition();
        }
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        PrepareForProgrammaticMove();

        yield return ExecuteMoveSequence(targetPosition, duration);

        FinalizeMove();
    }

    public void RegisterMoveCompleteCallback(Action callback) => _onMoveComplete = callback;

    private void PrepareForProgrammaticMove()
    {
        _isProgrammaticMove = true;
        _scaleController.Stop();
        _scaleController.StartScaleUp();
    }

    private IEnumerator ExecuteMoveSequence(Vector3 targetPosition, float duration)
    {
        yield return transform.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();
    }

    private void FinalizeMove()
    {
        _scaleController.StartScaleDown();
        _onMoveComplete?.Invoke();
        _isProgrammaticMove = false;
    }

    private void UpdateTilt()
    {
        CalculateXOffset();
        CalculateNormalizedOffset();
        CalculateTargetAngle();
        ApplyTilt();
    }

    private void CalculateXOffset() =>
        _xOffset = transform.position.x - _screenCenter.x;

    private void CalculateNormalizedOffset() =>
        _normalizedOffset = Mathf.Clamp(_xOffset / _tiltController.MaxDistance, -1f, 1f);

    private void CalculateTargetAngle() =>
        _targetAngle = -_normalizedOffset * _tiltController.MaxTiltAngle;

    private void ApplyTilt() =>
        _tiltController.ApplyTilt(_targetAngle);

    private bool HasPositionChanged() =>
        Vector3.Distance(transform.position, _basePosition) > _tiltController.PositionThreshold;

    private void UpdateBasePosition() =>
        _basePosition = transform.position;
}