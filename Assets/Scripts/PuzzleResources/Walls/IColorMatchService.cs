using UnityEngine;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.Walls
{
    public interface IColorMatchService
    {
        public bool Match(IColorModifiable other, out Color matchedColor);

        public void Reset();

        public void Initialize(IColorPrecision colorPrecision);
    }
}