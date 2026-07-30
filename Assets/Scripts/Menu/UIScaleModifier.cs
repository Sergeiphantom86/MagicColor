using System.Collections;
using UnityEngine;

namespace Menu
{
    public class UIScaleModifier : MonoBehaviour
    {
        [SerializeField] private float _multiplier;

        private ZoomChanger _zoomChanger;
        private float _startSize;
        private bool _isStandardSize;

        private void Awake()
        {
            _zoomChanger = new ZoomChanger();
            _startSize = transform.localScale.x;
            _isStandardSize = true;
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            SetSize();
        }

        private void Update()
        {
            _zoomChanger.ChangeLocation(SetSize);
        }

        private void SetSize()
        {
            if (_zoomChanger.IsMobileWithTallScreen() && _multiplier > 0 && _isStandardSize)
            {
                transform.localScale = Vector3.one * _multiplier;
                _isStandardSize = false;
            }
            else if (_isStandardSize == false && _zoomChanger.IsMobileWithTallScreen() == false)
            {
                transform.localScale = Vector3.one * _startSize;
                _isStandardSize = true;
            }
        }
    }
}