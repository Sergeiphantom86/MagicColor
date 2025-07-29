using UnityEngine;

public class ColorPrecision : IColorPrecision
{
    private readonly int _precision = 6;
    private readonly float _epsilon = 0.1f;

    public Color Reduce(Color color)
    {

        return new Color(RoundComponent(color.r), RoundComponent(color.g), RoundComponent(color.b));
    }

    public bool Match(Color firstColor, Color secondColor)
    {
        return Mathf.Abs(firstColor.r - secondColor.r) < _epsilon &&
               Mathf.Abs(firstColor.g - secondColor.g) < _epsilon &&
               Mathf.Abs(firstColor.b - secondColor.b) < _epsilon;
    }

    private float RoundComponent(float component)
    {
        return Mathf.Round(component * _precision) / _precision;
    }
}