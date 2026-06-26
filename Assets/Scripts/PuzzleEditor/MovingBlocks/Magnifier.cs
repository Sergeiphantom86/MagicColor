using System;
using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    public class Magnifier : MonoBehaviour
    {
        [SerializeField]
        private float _selectedScale;

        [SerializeField]
        private float _animationDuration;

        private Vector3 _originalScale;
        private Tween _scaleTween;
        private Transform _transform;

        public event Action Raised;

        public event Action Dropped;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _transform = transform;
        }

        public Tween Select()
        {
            _scaleTween = ChangeSize(_originalScale * _selectedScale);
            Raised?.Invoke();
            return _scaleTween;
        }

        public Tween Deselect()
        {
            _scaleTween = ChangeSize(_originalScale);
            Dropped?.Invoke();
            return _scaleTween;
        }

        public Tween ChangeSize(Vector3 scale)
        {
            return _transform.DOScale(scale, _animationDuration);
        }

        private void OnDestroy()
        {
            _scaleTween?.Kill();
        }
    }
}