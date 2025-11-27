using UnityEngine;

public class ZoomChanger
{
    private readonly Vector2 _referenceResolution = new(1014f, 570f);
    private readonly float _referenceAspect = 1014f / 570f;

    private float _screenSizeMultiplier;

    public float GetScreenSize(Camera camera)
    {
        float width = camera.scaledPixelWidth;
        float height = camera.scaledPixelHeight;
        float currentAspect = width / height;

        if (Mathf.Abs(currentAspect - _referenceAspect) < 0.1f)
        {
            _screenSizeMultiplier = width / _referenceResolution.x;
        }
        else if (currentAspect < _referenceAspect)
        {
            _screenSizeMultiplier = (height / _referenceResolution.y) * currentAspect;
        }
        else
        {
            _screenSizeMultiplier = width / _referenceResolution.x;
        }

        return _screenSizeMultiplier;
    }
}