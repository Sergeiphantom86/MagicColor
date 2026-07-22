using Menu;
using UnityEngine;

namespace RenderingCamera
{
    public class CameraPositionFitter : MonoBehaviour
    {
        private Camera _camera;
        private ZoomChanger _zoomChanger;
        private float _startPositionY;
        private float _mobilePositionX;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _zoomChanger = new ZoomChanger();
            _startPositionY = 12f;
            _mobilePositionX = -0.25f;

            SetInitialPosition();
        }

        private void SetInitialPosition()
        {
            Vector3 pos = transform.position;
            pos.y = _startPositionY;

            if (_zoomChanger.IsMobileWithTallScreen())
            {
                pos.x = _mobilePositionX;
            }

            transform.position = pos;
        }

        private void LateUpdate()
        {
            Vector3 pos = transform.position;

            if (_zoomChanger.IsMobileWithTallScreen())
            {
                pos.y = _startPositionY + _camera.orthographicSize * 0.4f;
                pos.x = _mobilePositionX;
            }
            else
            {
                pos.y = _startPositionY;
            }

            transform.position = pos;
        }
    }
}