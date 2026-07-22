using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public sealed class MaterialColorController
    {
        private readonly Material _material;
        private Color _originalColor;

        public MaterialColorController(Material material)
        {
            _material = material;
        }

        public Color CurrentColor => _material.color;

        public Color OriginalColor => _originalColor;

        public void SetColor(Color color)
        {
            _material.color = color;
        }

        public void SetOriginalColor(Color color)
        {
            _originalColor = color;
        }

        public void Restore(bool isRepainted)
        {
            _material.color = isRepainted
                ? _originalColor
                : Color.white;
        }
    }
}