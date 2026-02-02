using System.Collections;
using UnityEngine;

public class CameraFitToObject : MonoBehaviour
{
    [SerializeField] private float _referenceAspect;

    private float _maxFov;
    private float _zShift;
    private float _baseFov;
    private bool _isShifted;
    private float _lastWidth;
    private float _lastHeight;
    private int _angleDivider;
    private float _shiftExitFov;
    private float _shiftEnterFov;
    private float _currentAspect;
    private Camera _camera;
    private Vector3 _startPosition;
    private Coroutine _recalculateRoutine;
    private ZoomChanger _zoomChanger;

    private void Awake()
    {
        _maxFov = 155f;
        _zShift = 0.5f;
        _angleDivider = 2;
        _shiftExitFov = 110f;
        _shiftEnterFov = 115f;
        _camera = _camera != null ? _camera : Camera.main;
        _startPosition = transform.position;
        _zoomChanger = new ZoomChanger();
    }

    private void Start()
    {
        if (_zoomChanger.IsMobileWithTallScreen()) return;

        _baseFov = 33;

        SetScreenSizes();
        StartRecalculate();
    }

    private void Update()
    {
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
        {
            SetScreenSizes();
            StartRecalculate();
        }
    }

    private void SetScreenSizes()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
    }

    private void UpdateCameraFov()
    {
        _currentAspect = (float)Screen.width / Screen.height;

        float targetFov = GetTargetFov();
        _camera.fieldOfView = targetFov;

        HandleCameraShift(targetFov);
    }

    private float GetTargetFov()
    {
        if (_currentAspect >= _referenceAspect)
            return _baseFov;

        return Mathf.Clamp(GetVerticalFov(_currentAspect) * Mathf.Rad2Deg, _baseFov, _maxFov);
    }

    private void HandleCameraShift(float currentFov)
    {
        if (_isShifted == false && currentFov >= _shiftEnterFov)
        {
            ShiftCamera(true);
        }
        else if (_isShifted && currentFov <= _shiftExitFov)
        {
            ShiftCamera(false);
        }
    }

    private void ShiftCamera(bool shift)
    {
        Vector3 position = shift ? _startPosition : transform.position;

        position.z = shift
            ? _startPosition.z + _zShift
            : _startPosition.z;

        transform.position = position;
        _isShifted = shift;
    }

    private float GetBaseFovRad()
    {
        return _baseFov * Mathf.Deg2Rad;
    }

    private float GetHorizontalFov()
    {
        return _angleDivider * Mathf.Atan(Mathf.Tan(GetBaseFovRad() / _angleDivider) * _referenceAspect);
    }

    private float GetVerticalFov(float aspect)
    {
        return _angleDivider * Mathf.Atan(Mathf.Tan(GetHorizontalFov() / _angleDivider) / aspect);
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

        UpdateCameraFov();
    }
}