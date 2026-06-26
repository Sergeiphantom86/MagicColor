using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.Stars
{
    [RequireComponent(typeof(Image))]

    public class StarIndicator : MonoBehaviour
    {
        private Star _star;
        private Image _inactivePart;
        private float _duration;
        private float _minScale;
        private float _overshoot;
        private float _delay;

        private Tweener _currentTween;

        public bool IsActive { get; private set; }
        private void Awake()
        {
            _delay = 0.1f;
            _duration = 0.5f;
            _minScale = 0.2f;
            _overshoot = 1.5f;

            _inactivePart = GetComponent<Image>();
            IsActive = true;
        }

        private void Start()
        {
            _star = GetComponentInChildren<Star>(true);

            if (_star == null)
            {
                Debug.LogError("Star == null");
                return;
            }

            _inactivePart.enabled = true;

            _star.SetActive(false);
        }

        public void TurnOn()
        {
            if (_star == null)
            {
                Debug.LogError("Star == null");
                return;
            }

            _currentTween?.Kill();

            _star.SetActive(true);

            _star.transform.localScale = Vector3.one * _minScale;

            _currentTween = _star
            .transform.DOScale(Vector3.one, _duration)
            .SetDelay(_delay)
            .SetEase(Ease.OutBack, overshoot: _overshoot);
        }

        public void SetInactive()
        {
            _currentTween?.Kill();

            if (_inactivePart == null)
            return;

            IsActive = false;

            if (_star != null)
            {
                _currentTween?.Kill();

                _currentTween = _star
                .transform.DOScale(Vector3.zero, _duration)
                .SetDelay(_delay)
                .SetEase(Ease.OutBack, overshoot: _overshoot);
            }
        }
    }
}