using DG.Tweening;
using System;
using UnityEngine;

public class ParticleAnimation : MonoBehaviour
{
    private Vector3 _randomPosition;
    private Vector3 _targetPosition;
    private Settings _settings;

    private event Action _onCompleteCallback;

    public struct Settings
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

    public void Initialize(Vector3 randomPosition, Vector3 targetPosition, Settings settings, Action onComplete = null)
    {
        _randomPosition = randomPosition;
        _targetPosition = targetPosition;
        _settings = settings;
        _onCompleteCallback = onComplete;

        RunAnimation();
    }

    private void RunAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(transform.DOScale(UnityEngine.Random.Range(_settings.MinScale, _settings.MaxScale), _settings.ScaleUpDuration));
        sequence.Join(transform.DOMove(_randomPosition, _settings.MoveToRandomDuration));
        sequence.Join(transform.DORotate(new Vector3(0, 0, 360f), _settings.MoveToRandomDuration, RotateMode.FastBeyond360));

        sequence.AppendCallback(MoveToTarget);
    }

    private void MoveToTarget()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(GetMoveInitialSection(), GetDuration()).SetEase(Ease.Linear));

        sequence.Append(transform.DOMove(_targetPosition, _settings.MoveToTargetDuration * (1 - _settings.FirstPhaseRatio))
            .SetEase(Ease.InQuad));

        sequence.Join(transform.DOScale(0f, _settings.MoveToTargetDuration * (1 - _settings.FirstPhaseRatio)));

        sequence.OnComplete(() =>
        {
            _onCompleteCallback?.Invoke();

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
}