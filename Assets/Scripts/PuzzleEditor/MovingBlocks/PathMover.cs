using System;
using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    public class PathMover : MonoBehaviour
    {
        [SerializeField] private float _durationToWaypoint = 0.1f;
        [SerializeField] private float _durationToEnd = 0.2f;

        private float _scaleDownDuration;
        private float _scaleUpDuration;
        private float _scaleReturnDuration;
        private float _scaleDownFactor;
        private float _scaleUpFactor;
        private float _period;
        private Sequence _pathSequence;
        private Vector3 _originalScale;

        private void Awake()
        {
            _scaleDownDuration = 0.05f;
            _scaleUpDuration = 0.05f;
            _scaleReturnDuration = 0.1f;
            _scaleDownFactor = 0.8f;
            _scaleUpFactor = 1.2f;
            _period = 0.3f;
            _originalScale = transform.localScale;
        }

        private void OnDestroy()
        {
            if (_pathSequence != null && _pathSequence.IsActive())
            {
                _pathSequence.Kill();
            }
        }

        public void Move(Vector3 waypoint, Vector3 endPoint, Action onComplete = null)
        {
            _originalScale = transform.localScale;

            if (_pathSequence != null && _pathSequence.IsActive())
                _pathSequence.Kill();

            _pathSequence = DOTween.Sequence();

            _pathSequence.Append(transform
            .DOScale(_originalScale * _scaleDownFactor, _scaleDownDuration)
            .SetEase(Ease.OutQuad));

            _pathSequence.Append(transform
            .DOScale(_originalScale * _scaleUpFactor, _scaleUpDuration)
            .SetEase(Ease.OutBack));

            AddMovePoint(_pathSequence, waypoint, _durationToWaypoint);
            AddMovePoint(_pathSequence, endPoint, _durationToEnd);

            _pathSequence.Append(transform
            .DOScale(_originalScale, _scaleReturnDuration)
            .SetEase(Ease.OutElastic, _scaleDownFactor, _period));

            _pathSequence.OnComplete(() => onComplete?.Invoke());
            _pathSequence.Play();
        }

        private void AddMovePoint(Sequence sequence, Vector3 targetPosition, float duration)
        {
            sequence.Append(transform.DOMove(targetPosition, duration));
        }
    }
}