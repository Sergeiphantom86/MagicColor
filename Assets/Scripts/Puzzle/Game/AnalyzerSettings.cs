using UnityEngine;

[System.Serializable]
public class AnalyzerSettings
{
    public IColorPrecision ColorPrecision;
    public Sprite Sprites;
    public int DefaultSpriteIndex;
    public bool IgnoreTransparent = true;
    [Range(0, 1)] public float IgnoredAlpha;
}