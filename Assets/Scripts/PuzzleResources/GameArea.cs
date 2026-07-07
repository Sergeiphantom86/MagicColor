using DG.Tweening;
using UnityEngine;
using YG;

namespace PuzzleResources
{
    public class GameArea : MonoBehaviour
    {
        private const float PositionX = 50;
        private const float DurationDivider = 10;

        private Rotator _rotation;
        private Tween _moveTween;

        private void Awake()
        {
            _rotation = GetComponent<Rotator>();
        }

        private void Start()
        {
            YG2.SaveProgress();
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
        }

        public void Return(float duration)
        {
            MoveX(duration);
        }

        public void StartRotation()
        {
            _rotation = GetComponent<Rotator>();

            _rotation.StartRotation();
        }

        private void MoveX(float duration)
        {
            _moveTween?.Kill();
            _moveTween = transform
            .DOMoveX(GetPositionX(), GetDuration(duration))
            .SetEase(Ease.Linear)
            .OnComplete(() => gameObject.SetActive(false));
        }

        private float GetPositionX()
        {
            return transform.position.x - PositionX;
        }

        private float GetDuration(float duration)
        {
            return duration / DurationDivider;
        }
    }
}