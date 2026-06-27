using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.UI
{
    public class MoverUI
    {
        private Sequence _sequence;

        public Sequence EnableMotionAnimation(
            RectTransform elementRect,
            float duration,
            RectTransform canvasRect,
            float normalizedX,
            float normalizedY, float _ = 0)
        {
            if (ValidateInput(elementRect, canvasRect, duration) == false)
                return CreateEmptySequence();

            CreateSequence();

            Vector2 targetPosition = GetTargetPosition(canvasRect, normalizedX, normalizedY);
            _sequence.Join(elementRect.DOAnchorPos(targetPosition, duration).SetEase(Ease.OutBack));

            Play();
            return _sequence;
        }

        public Sequence EnableAnimationResizing(
            RectTransform elementRect,
            float duration,
            float normalizedScaleX = 1,
            float normalizedScaleY = 1,
            float normalizedScaleZ = 1)
        {
            if (ValidateInput(elementRect, duration) == false)
                return CreateEmptySequence();

            CreateSequence();

            Vector3 targetScale = GetTargetScale(elementRect, normalizedScaleX, normalizedScaleY, normalizedScaleZ);
            _sequence.Join(elementRect.DOScale(targetScale, duration).SetEase(Ease.OutBack));

            Play();
            return _sequence;
        }

        public void Play()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Play();
            }
        }

        public bool IsActive()
        {
            return _sequence != null && _sequence.IsActive();
        }

        private bool ValidateInput(RectTransform elementRect, float duration)
        {
            if (elementRect == null)
            {
                Debug.LogError("MoverUI: Element RectTransform is null!");
                return false;
            }

            if (duration <= 0)
            {
                Debug.LogWarning("MoverUI: Duration should be positive! Using default value 0.1f");
                return false;
            }

            return true;
        }

        private bool ValidateInput(RectTransform elementRect, RectTransform canvasRect, float duration)
        {
            if (ValidateInput(elementRect, duration) == false)
                return false;

            if (canvasRect == null)
            {
                Debug.LogError("MoverUI: Canvas RectTransform is null!");
                return false;
            }

            return true;
        }

        private Vector2 GetTargetPosition(RectTransform canvasRect, float normalizedX, float normalizedY)
        {
            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedY = Mathf.Clamp01(normalizedY);

            return new Vector2(
                canvasRect.sizeDelta.x * (normalizedX - 0.5f),
                canvasRect.sizeDelta.y * (normalizedY - 0.5f));
        }

        private Vector3 GetTargetScale(
            RectTransform elementRect,
            float normalizedScaleX,
            float normalizedScaleY,
            float normalizedScaleZ)
        {
            normalizedScaleX = Mathf.Max(0.01f, normalizedScaleX);
            normalizedScaleY = Mathf.Max(0.01f, normalizedScaleY);
            normalizedScaleZ = Mathf.Max(0.01f, normalizedScaleZ);

            return new Vector3(
                elementRect.localScale.x * normalizedScaleX,
                elementRect.localScale.y * normalizedScaleY,
                elementRect.localScale.z * normalizedScaleZ);
        }

        private void CreateSequence()
        {
            if (IsActive())
            {
                _sequence.Kill();
            }

            _sequence = DOTween.Sequence();
        }

        private Sequence CreateEmptySequence()
        {
            Sequence emptySequence = DOTween.Sequence();
            emptySequence.Complete();
            return emptySequence;
        }
    }
}