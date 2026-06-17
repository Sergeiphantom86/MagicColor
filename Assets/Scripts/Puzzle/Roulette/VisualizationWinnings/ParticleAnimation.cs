using System;
using DG.Tweening;
using UnityEngine;

public class ParticleAnimation : MonoBehaviour
{
    private Vector3 _randomPosition;
    private Vector3 _targetPosition;
    private Settings _settings;
    private Sequence _firstSequence;
    private Sequence _secondSequence;

    private event Action OnCompleteCallback;

    private void OnDestroy()
    {
        _firstSequence?.Kill();
        _secondSequence?.Kill();
        transform.DOKill();
    }

    public void Initialize(Vector3 randomPosition, Vector3 targetPosition, Settings settings, Action onComplete = null)
    {
        _randomPosition = randomPosition;
        _targetPosition = targetPosition;
        _settings = settings;
        OnCompleteCallback = onComplete;

        RunAnimation();
    }

    private void RunAnimation()
    {
        _firstSequence = DOTween.Sequence();

        _firstSequence.Join(transform.DOScale(UnityEngine.Random.Range(_settings.MinScale, _settings.MaxScale), _settings.ScaleUpDuration));
        _firstSequence.Join(transform.DOMove(_randomPosition, _settings.MoveToRandomDuration));
        _firstSequence.Join(transform.DORotate(new Vector3(0, 0, 360f), _settings.MoveToRandomDuration, RotateMode.FastBeyond360));

        _firstSequence.AppendCallback(MoveToTarget);
    }

    private void MoveToTarget()
    {
        _secondSequence = DOTween.Sequence();

        _secondSequence.Append(transform.DOMove(GetMoveInitialSection(), GetDuration()).SetEase(Ease.Linear));

        _secondSequence.Append(transform.DOMove(_targetPosition, _settings.MoveToTargetDuration * (1 - _settings.FirstPhaseRatio))
            .SetEase(Ease.InQuad));

        _secondSequence.Join(transform.DOScale(0f, _settings.MoveToTargetDuration * (1 - _settings.FirstPhaseRatio)));

        _secondSequence.OnComplete(() =>
        {
            OnCompleteCallback?.Invoke();
            transform.DOKill();
            Destroy(gameObject);
        });
    }

    private Vector3 GetMoveInitialSection()
    {
        return Vector3.Lerp(transform.position, _targetPosition, _settings.FirstPhaseRatio);
    }

    private float GetDuration()
    {
        return _settings.MoveToTargetDuration * _settings.FirstPhaseRatio;
    }

    public readonly struct Settings
    {
        public readonly float MinScale;
        public readonly float MaxScale;
        public readonly float ScaleUpDuration;
        public readonly float MoveToRandomDuration;
        public readonly float MoveToTargetDuration;
        public readonly float FirstPhaseRatio;

        public Settings(float minScale, float maxScale, float scaleUpDuration, float moveToRandomDuration, float moveToTargetDuration, float firstPhaseRatio)
        {
            MinScale = minScale;
            MaxScale = maxScale;
            ScaleUpDuration = scaleUpDuration;
            MoveToRandomDuration = moveToRandomDuration;
            MoveToTargetDuration = moveToTargetDuration;
            FirstPhaseRatio = firstPhaseRatio;
        }
    }
}