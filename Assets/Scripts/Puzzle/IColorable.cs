using UnityEngine;

public interface IColorable
{
    void SetColor(Color color);
    void SetActive(bool state);
    Color GetColor();
}