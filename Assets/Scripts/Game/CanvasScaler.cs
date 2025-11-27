using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasScaler : MonoBehaviour
{
    private Canvas _canvas;
    private Camera _camera;
    private ZoomChanger _zoomChanger;

    private void Awake()
    {
        _camera = Camera.main;
        _canvas = GetComponent<Canvas>();
        _zoomChanger = new ZoomChanger();
    }

    private void Start()
    {
        _canvas.scaleFactor *= _zoomChanger.GetScreenSize(_camera);
    }
}