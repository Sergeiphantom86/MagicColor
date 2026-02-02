using System.Collections;
using UnityEngine;

public class FitToCameraBottomAndWidth : MonoBehaviour
{
    private Camera _camera;
    private float _baseOrthoSize;
    private float _startPositionY;
    private float _lastWidth;
    private float _lastHeight;
    private Vector3 _startPosition;
    private Coroutine _recalculateRoutine;
    private ZoomChanger _zoomChanger;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _zoomChanger = new ZoomChanger();
        _startPositionY = 12;
        _baseOrthoSize = 6;
        _baseOrthoSize /= 2;
        SetStartPosition(_startPositionY);

        if (_zoomChanger.IsMobileWithTallScreen()) return;
    }

    private void Start()
    {
        SetScreenSizes();
        StartRecalculate();
    }

    private void SetScreenSizes()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
    }

    private void StartRecalculate()
    {
        if (_recalculateRoutine != null)
            StopCoroutine(_recalculateRoutine);

        _recalculateRoutine = StartCoroutine(RecalculateDelayed());
    }

    private void UpdateCameraOrthoSize()
    {
        if (_zoomChanger.IsMobileWithTallScreen())
        {
            _camera.orthographic = true;
            _camera.orthographicSize = GetTargetOrthogonal();
            SetStartPosition(_startPosition.y + _camera.orthographicSize * 2);
        }
    }

    private float GetTargetOrthogonal()
    {
        return _baseOrthoSize + (2f / GetCurrentAspect());
    }

    private float GetCurrentAspect()
    {
        return _lastWidth / _lastHeight;
    }

    private void SetStartPosition(float startPosition)
    {
        _startPosition = new (transform.position.x, startPosition, transform.position.z);

        transform.position = _startPosition;
    }

    private IEnumerator RecalculateDelayed()
    {
        yield return new WaitForEndOfFrame();
  
        UpdateCameraOrthoSize();
    }
}