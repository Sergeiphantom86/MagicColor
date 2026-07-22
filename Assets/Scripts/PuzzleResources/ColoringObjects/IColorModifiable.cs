using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public interface IColorModifiable
    {
        public Color GetColor();

        public void SetColor(Color color);
    }
}