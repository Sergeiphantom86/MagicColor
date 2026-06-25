using UnityEngine;

namespace PuzzleEditor
{
    public interface IColorPrecision
    {
        Color Reduce(Color original);

        bool Match(Color firstColor, Color secondColor);
    }
}