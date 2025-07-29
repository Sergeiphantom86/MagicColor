using UnityEngine;

public interface IColorPrecision
{
    Color Reduce(Color original);
    bool Match(Color firstColor, Color secondColor);
}