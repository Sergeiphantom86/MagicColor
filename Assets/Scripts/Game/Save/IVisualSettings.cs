using UnityEngine;

public interface IVisualSettings
{
    public void SetNewSprite(Sprite sprite);
    public void SetCurrentSprite(Sprite sprite);
    public void MakeTransparent(bool isTransparency);
}