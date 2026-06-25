using UnityEngine;

public class ColorPrecision : IColorPrecision
{
    private const int PrecisionSteps = 6;
    private const float ColorEpsilon = 0.02f;
    private const float AlphaThreshold = 0.9f;

    public Color Reduce(Color color)
    {
        return new Color(
            Round(color.r),
            Round(color.g),
            Round(color.b),
            color.a);
    }

    public bool Match(Color firstColor, Color secondColor)
    {
        if (firstColor.a < AlphaThreshold || secondColor.a < AlphaThreshold)
            return false;

        firstColor = Reduce(firstColor);
        secondColor = Reduce(secondColor);

        return Mathf.Abs(firstColor.r - secondColor.r) <= ColorEpsilon &&
               Mathf.Abs(firstColor.g - secondColor.g) <= ColorEpsilon &&
               Mathf.Abs(firstColor.b - secondColor.b) <= ColorEpsilon;
    }

    private float Round(float value)
    {
        return Mathf.Round(value * PrecisionSteps) / PrecisionSteps;
    }
}