using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.UI
{
    [RequireComponent(typeof(RectTransform))]

    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] private float _positionX;
        [SerializeField] private float _positionY;

        private float _scale;
        private float _duration;
        private MoverUI _moverUI;
        private Sequence _sequence;
        private RectTransform _rectTransform;

        private Vector3 _startPosition;

        private void Awake()
        {
            _scale = 0.1f;
            _duration = 1;
            _moverUI = new MoverUI();
            _sequence = DOTween.Sequence();
            _startPosition = transform.position;
            _rectTransform = GetComponent<RectTransform>();
        }

        public Sequence Move(RectTransform canvasRect)
        {
            return GetSequence(canvasRect, 0, _positionY, 0);
        }

        public void Return(RectTransform canvasRect)
        {
            GetSequence(canvasRect, _startPosition.x, _startPosition.y, 0);
        }

        public void Increase()
        {
            _moverUI.EnableAnimationResizing(_rectTransform, _duration, _scale, _scale);
        }

        private Sequence GetSequence(RectTransform canvasRect, float positionX, float positionY, float positionZ)
        {
            return _moverUI.EnableMotionAnimation(_rectTransform, _duration, canvasRect, positionX, positionY, positionZ);
        }

        private void OnDestroy()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Kill();
            }
        }
    }
}