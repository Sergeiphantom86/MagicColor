using UnityEngine;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.Walls
{
    [RequireComponent(typeof(IColorModifiable), typeof(IDisable), typeof(IRepaintable))]
    
    public class ColorMatchService : MonoBehaviour, IColorMatchService
    {
        private IColorModifiable _colorable;
        private IColorPrecision _precision;
        private IDisable _disable;
        private IRepaintable _repaintable;

        public void Reset()
        {
            _disable.Disable();
        }

        public void Initialize(IColorPrecision precision)
        {
            _precision = precision;
            _colorable = GetComponent<IColorModifiable>();
            _disable = GetComponent<IDisable>();
            _repaintable = GetComponent<IRepaintable>();
        }

        public bool Match(IColorModifiable other, out Color matchedColor)
        {
            matchedColor = default;

            _repaintable.AssignOriginal();
            
            Color otherColor = other.GetColor();

            if (otherColor == Color.white)
                return false;

            if (_precision.Match(_colorable.GetColor(), otherColor) == false)
                return false;

            matchedColor = otherColor;
            return true;
        }
    }
}