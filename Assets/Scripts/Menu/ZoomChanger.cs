using System;
using UnityEngine;
using YG;

namespace Menu
{
    public class ZoomChanger
    {
        private readonly Vector2 _referenceResolution = new(1014f, 570f);
        private readonly float _referenceAspect = 1014f / 570f;
        private readonly float _mobileAspectRatio = 1.5f;

        private float _currentAspect;
        private float _width;
        private float _height;
        private float _lastWidth;
        private float _lastHeight;

        public float MobileAspectRatio => _mobileAspectRatio;

        public float GetScreenSize(Camera camera)
        {
            _width = camera.scaledPixelWidth;
            _height = camera.scaledPixelHeight;

            _currentAspect = _width / _height;

            float screenSizeMultiplier = 0;

            if (Mathf.Abs(_currentAspect - _referenceAspect) < 0.1f)
            {
                screenSizeMultiplier = _width / _referenceResolution.x;
            }
            else if (_currentAspect < _referenceAspect)
            {
                screenSizeMultiplier = _height / _referenceResolution.y * _currentAspect;
            }

            return screenSizeMultiplier;
        }

        public float GetAspect()
        {
            return (float)Screen.height / Screen.width;
        }

        public void ChangeLocation(Action action)
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;

                action();
            }
        }

        public bool IsMobileWithTallScreen()
        {
            return YG2.envir.isMobile && IsMobileLike();
        }

        private bool IsMobileLike()
        {
            return GetAspect() > MobileAspectRatio;
        }
    }
}