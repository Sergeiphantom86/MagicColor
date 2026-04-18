using UnityEngine;

public interface IColorable
{
    public void SetColor(Color color);
    public void SetActive(bool state);
    public Color GetColor();
    public void InstallRepainted();
    public void AssignOriginal();
    public void Disable();
    public void SetAlpha(float alpha);
    public bool IsRepainted { get; }
    public void SetRenderQueue();
    public void SetStartRenderQueueSelectedItem();
    public void SetRenderQueueSelectedItem();
}