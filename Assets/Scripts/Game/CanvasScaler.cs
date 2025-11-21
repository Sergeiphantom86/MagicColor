using UnityEngine;

public class CanvasScaler : MonoBehaviour
{
    private Canvas _canvas;
    private Camera _camera;
    private float _sizeDivider;
    private float _screenSizeMultiplier;

    private void Awake()
    {
        _sizeDivider = 1000;
        _camera = Camera.main;
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        //AdjustScreenSize();
    }

    //private void AdjustScreenSize()
    //{
    //    _screenSizeMultiplier = _camera.scaledPixelWidth;
    //    _screenSizeMultiplier /= _sizeDivider;

    //    _camera.scaleFactor *= _screenSizeMultiplier;
    //}
}