using UnityEngine;
namespace PuzzleEditor.Walls
{

public class ColorMatchService : MonoBehaviour, IColorMatchService
{
    private IColorable _colorable;
    private IColorPrecision _precision;

    public void Initialize(IColorPrecision precision)
    {
        _precision = precision;
        _colorable = GetComponent<IColorable>();
    }

    public bool Match(IColorable other, out Color matchedColor)
    {
        matchedColor = default;

        _colorable.AssignOriginal();

        Color otherColor = other.GetColor();

        if (otherColor == Color.white)
            return false;

        if (_precision.Match(_colorable.GetColor(), otherColor) == false)
            return false;

        matchedColor = otherColor;
        return true;
    }

    public void Reset()
    {
        _colorable.Disable();
    }
}

}