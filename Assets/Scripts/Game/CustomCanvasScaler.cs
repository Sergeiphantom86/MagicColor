using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CustomCanvasScaler : MonoBehaviour
{
    [Header("Mobile")]
    [SerializeField] private float _mobileScaleMultiplier;

    private Canvas _canvas;
    private Camera _camera;
    private ZoomChanger _zoomChanger;

    private float _lastWidth;
    private float _lastHeight;
    private Coroutine _recalculateRoutine;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _zoomChanger = new ZoomChanger();
        _camera = _camera != null ? _camera : Camera.main;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        StartRecalculate();
    }

    private void Update()
    {
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            StartRecalculate();
        }
    }

    private void StartRecalculate()
    {
        if (_recalculateRoutine != null)
            StopCoroutine(_recalculateRoutine);

        _recalculateRoutine = StartCoroutine(RecalculateDelayed());
    }

    private IEnumerator RecalculateDelayed()
    {
        yield return null;
        yield return null;

        float scale = _zoomChanger.GetScreenSize(_camera);

        if (scale <= 0f)
            yield break;

        if (_zoomChanger.IsMobileWithTallScreen() == false)
            yield break;

        _canvas.scaleFactor = scale * _mobileScaleMultiplier;
    }
}