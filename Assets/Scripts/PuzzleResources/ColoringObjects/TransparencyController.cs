using DG.Tweening;
using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public sealed class TransparencyController
    {
        private readonly Material _material;
        private readonly float _fadeDuration;
        private readonly float _targetTransparency;

        public TransparencyController(
            Material material,
            float fadeDuration,
            float targetTransparency)
        {
            _material = material;
            _fadeDuration = fadeDuration;
            _targetTransparency = targetTransparency;
        }

        public Tween FadeTo(Color targetColor)
        {
            Color endColor = new(
                targetColor.r,
                targetColor.g,
                targetColor.b,
                _targetTransparency);

            return DOTween.To(() =>
            _material.color, value =>
            _material.color = value, endColor, _fadeDuration)
                .SetEase(Ease.Linear);
        }

        public void MakeTransparent()
        {
            Color color = _material.color;
            color.a = _targetTransparency;
            _material.color = color;
        }

        public void SetAlpha(float alpha)
        {
            Color color = _material.color;
            color.a = alpha;
            _material.color = color;
        }
    }
}