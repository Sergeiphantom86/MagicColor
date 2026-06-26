using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class CarouselLayoutCalculator
    {
        private const float Half = 0.5f;

        private readonly RectTransform[] _buttons;
        private readonly Vector2[] _originalPositions;
        private readonly float _centerScale;
        private readonly float _sideScale;
        private readonly float _distanceFromCenter;

        public CarouselLayoutCalculator(
        RectTransform[] buttons,
        Vector2[] originalPositions,
        float centerScale,
        float sideScale,
        float distanceFromCenter
        )
        {
            _buttons = buttons;
            _originalPositions = originalPositions;
            _centerScale = centerScale;
            _sideScale = sideScale;
            _distanceFromCenter = distanceFromCenter;
        }

        public float GetTargetPositionX(int buttonIndex, int centerIndex)
        {
            if (buttonIndex == centerIndex)
            return GetCenterX(centerIndex);

            return GetAccumulatedOffsetX(buttonIndex, centerIndex);
        }

        private float GetCenterX(int centerIndex)
        {
            return _originalPositions[centerIndex].x;
        }

        private float GetAccumulatedOffsetX(int buttonIndex, int centerIndex)
        {
            float direction = GetDirection(buttonIndex, centerIndex);
            float positionX = GetCenterX(centerIndex);

            foreach (int stepIndex in GetSteps(centerIndex, buttonIndex))
            {
                positionX +=
                direction * GetStepDistance(stepIndex, stepIndex + (int)direction, centerIndex);
            }

            return positionX;
        }

        private IEnumerable<int> GetSteps(int from, int to)
        {
            int step = from < to ? 1 : -1;

            for (int i = from; i != to; i += step)
            yield return i;
        }

        private float GetStepDistance(int fromIndex, int toIndex, int centerIndex)
        {
            RectTransform from = _buttons[fromIndex];
            RectTransform to = _buttons[toIndex];

            float fromScale = GetScaleForIndex(fromIndex, centerIndex);
            float toScale = GetScaleForIndex(toIndex, centerIndex);

            float fromWidth = from.rect.width * fromScale;
            float toWidth = to.rect.width * toScale;

            return fromWidth * Half + toWidth * Half + _distanceFromCenter;
        }

        private float GetScaleForIndex(int index, int centerIndex)
        {
            return index == centerIndex ? _centerScale : _sideScale;
        }

        private float GetDirection(int buttonIndex, int centerIndex)
        {
            return Mathf.Sign(buttonIndex - centerIndex);
        }
    }
}