using UnityEngine;

public interface IColorable
{
    public void SetColor(Color color);
    public void SetActive(bool state);
    public Color GetColor();
    public void InstallRepainted();
    public void AssignOriginal();
    public void Disable();
    public void SetAlpha(Color color, float alpha);
}