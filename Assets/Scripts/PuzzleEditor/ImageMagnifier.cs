using Menu;
using UnityEngine;

namespace PuzzleEditor
{
    public class ImageMagnifier : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _positoonZ;

        [SerializeField]
        private float _multiplier;

        private float _lastWidth;
        private float _lastHeight;
        private float _startSize;
        private bool _isStandardSize;
        private Camera _camera;
        private Vector3 _startPositoonZ;
        private ZoomChanger _zoomChanger;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _zoomChanger = new ZoomChanger();
            _rectTransform = GetComponent<RectTransform>();
            _startPositoonZ = _rectTransform.localPosition;
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
            if (
                _zoomChanger.IsMobileWithTallScreen()
                && _multiplier > 0
                && _isStandardSize
                && _camera.fieldOfView > 116
            )
            {
                transform.localScale = Vector3.one * _multiplier;
                _rectTransform.position = _positoonZ;
                _isStandardSize = false;
            }
            else if (
                _isStandardSize == false
                && _zoomChanger.IsMobileWithTallScreen() == false
                && _camera.fieldOfView < 116
            )
            {
                transform.localScale = Vector3.one * _startSize;
                _rectTransform.position = _startPositoonZ;
                _isStandardSize = true;
            }
        }
    }
}