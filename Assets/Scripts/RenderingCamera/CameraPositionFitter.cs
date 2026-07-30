using Menu;
using UnityEngine;

namespace RenderingCamera
{
    public class CameraPositionFitter : MonoBehaviour
    {
        private const float VerticalOffsetRatio = 0.4f;
        private const float StartPositionY = 12f;
        private const float MobilePositionX = -0.25f;

        private Camera _camera;
        private ZoomChanger _zoomChanger;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _zoomChanger = new ZoomChanger();

            SetInitialPosition();
        }

        private void LateUpdate()
        {
            Vector3 position = transform.position;

            if (_zoomChanger.IsMobileWithTallScreen())
            {
                position.y = StartPositionY + _camera.orthographicSize * VerticalOffsetRatio;
                position.x = MobilePositionX;
            }
            else
            {
                position.y = StartPositionY;
            }

            transform.position = position;
        }

        private void SetInitialPosition()
        {
            Vector3 pos = transform.position;
            pos.y = StartPositionY;

            if (_zoomChanger.IsMobileWithTallScreen())
            {
                pos.x = MobilePositionX;
            }

            transform.position = pos;
        }
    }
}