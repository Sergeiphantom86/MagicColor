using UnityEngine;
using DG.Tweening;
using System;

public class PathMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Ease _moveEase = Ease.InOutBounce;
    [SerializeField] private float _durationToWaypoint = 0.1f;
    [SerializeField] private float _durationToEnd = 0.2f;

    [Header("Scale Animation Settings")]
    private float _scaleDownDuration = 0.05f;
    private float _scaleUpDuration = 0.05f;
    private float _scaleReturnDuration = 0.1f;
   private float _scaleDownFactor = 0.8f; 
    private float _scaleUpFactor = 1.2f; 
    private Sequence _pathSequence;
    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Move(Vector3 waypoint, Vector3 endPoint, Action onComplete = null)
    {
        _originalScale = transform.localScale;
        if (_pathSequence != null && _pathSequence.IsActive()) _pathSequence.Kill();

        _pathSequence = DOTween.Sequence();
        _pathSequence.Append(transform.DOScale(_originalScale * _scaleDownFactor, _scaleDownDuration).SetEase(Ease.OutQuad));
        _pathSequence.Append(transform.DOScale(_originalScale * _scaleUpFactor, _scaleUpDuration).SetEase(Ease.OutBack));

        AddMovePoint(_pathSequence, waypoint, _durationToWaypoint);
        AddMovePoint(_pathSequence, endPoint, _durationToEnd);

        _pathSequence.Append(transform.DOScale(_originalScale, _scaleReturnDuration).SetEase(Ease.OutElastic, 0.8f, 0.3f));

        _pathSequence.OnComplete(() => onComplete?.Invoke());
        _pathSequence.Play();
    }

    private void AddMovePoint(Sequence sequence, Vector3 targetPosition, float duration)
    {
        sequence.Append(transform.DOMove(targetPosition, duration));
    }

    private void OnDestroy()
    {
        if (_pathSequence != null && _pathSequence.IsActive())
        {
            _pathSequence.Kill();
        }
    }
}