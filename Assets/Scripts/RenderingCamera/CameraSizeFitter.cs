using Menu;
using System.Collections;
using UnityEngine;

namespace RenderingCamera
{
    public class CameraSizeFitter : MonoBehaviour
    {
        private Camera _camera;
        private ZoomChanger _zoomChanger;
        private float _baseOrthoSize;
        private float _lastWidth;
        private float _lastHeight;
        private Coroutine _recalculateRoutine;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _zoomChanger = new ZoomChanger();
            _baseOrthoSize = 3f;
        }

        private void Start()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            StartRecalculate();
        }

        private void StartRecalculate()
        {
            if (_recalculateRoutine != null)
                StopCoroutine(_recalculateRoutine);

            _recalculateRoutine = StartCoroutine(RecalculateDelayed());
        }

        private IEnumerator RecalculateDelayed()
        {
            yield return new WaitForEndOfFrame();
            UpdateCameraSize();
        }

        private void UpdateCameraSize()
        {
            if (_zoomChanger.IsMobileWithTallScreen() == false)
                return;

            _camera.orthographic = true;
            _camera.orthographicSize = GetTargetOrthogonal();
        }

        private float GetTargetOrthogonal()
        {
            return _baseOrthoSize + (1f / GetCurrentAspect());
        }

        private float GetCurrentAspect()
        {
            return _lastWidth / _lastHeight;
        }
    }
}