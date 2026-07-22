using Menu;
using UnityEngine;

namespace PuzzleResources
{
    public class ImageMagnifier : MonoBehaviour
    {
        private const float ForThreshold = 116f;

        [SerializeField] private Vector3 _positionZ;
        [SerializeField] private float _multiplier;

        private float _lastWidth;
        private float _lastHeight;
        private float _startSize;
        private bool _isStandardSize;
        private Camera _camera;
        private Vector3 _startPositionZ;
        private ZoomChanger _zoomChanger;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _zoomChanger = new ZoomChanger();
            _rectTransform = GetComponent<RectTransform>();
            _startPositionZ = _rectTransform.localPosition;
            _startSize = 0.3f;
            _camera = Camera.main;
            _isStandardSize = true;
        }

        private void Update()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;

                SetSize();
            }
        }

        private void SetSize()
        {
            if (_zoomChanger.IsMobileWithTallScreen() &&
                _multiplier > 0 &&
                _isStandardSize &&
                _camera.fieldOfView > ForThreshold)
            {
                transform.localScale = Vector3.one * _multiplier;
                _rectTransform.position = _positionZ;
                _isStandardSize = false;
            }
            else if (_isStandardSize == false &&
                _zoomChanger.IsMobileWithTallScreen() == false &&
                _camera.fieldOfView < ForThreshold)
            {
                transform.localScale = Vector3.one * _startSize;
                _rectTransform.position = _startPositionZ;
                _isStandardSize = true;
            }
        }
    }
}