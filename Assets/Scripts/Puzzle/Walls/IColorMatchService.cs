using UnityEngine;

public interface IColorMatchService
{
    public bool Match(IColorable other, out Color matchedColor);
    public void Reset();

    public void Initialize(IColorPrecision colorPrecision);
}