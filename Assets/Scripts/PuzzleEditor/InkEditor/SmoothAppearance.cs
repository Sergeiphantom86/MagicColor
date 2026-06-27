using System;
using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.InkEditor
{
    [RequireComponent(typeof(SmoothMoveToTarget), typeof(Drop))]

    public class SmoothAppearance : MonoBehaviour
    {
        private float _duration;
        private float _durationDeletion;
        private bool _useScale;
        private bool _disableOnStart;
        private Vector3 _originalScale;
        private SmoothMoveToTarget _smoothMoveToTarget;
        private Sequence _sequence;
        private Drop _drop;

        private void Awake()
        {
            _duration = 0.8f;
            _durationDeletion = 0.4f;
            _useScale = true;
            _disableOnStart = true;
            _originalScale = new Vector3(0.2f, 0.2f, 0.2f);
            _drop = GetComponent<Drop>();
            _smoothMoveToTarget = GetComponent<SmoothMoveToTarget>();

            if (_disableOnStart && _useScale)
                transform.localScale = Vector3.zero;
        }

        private void OnEnable()
        {
            Show();
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        public void Hide()
        {
            _drop.PlaySoundSpawn();

            CreateSizeChangeSequence(Vector3.zero, _durationDeletion, () => gameObject.SetActive(false));
        }

        private void Show()
        {
            gameObject.SetActive(true);

            CreateSizeChangeSequence(_originalScale, _duration, () => _smoothMoveToTarget.BeginMovement());
        }

        private void CreateSizeChangeSequence(Vector3 scale, float duration, Action action = null)
        {
            if (_useScale == false)
                return;

            _sequence?.Kill();

            _sequence = DOTween.Sequence();
            _sequence.Join(transform.DOScale(scale, duration).SetEase(Ease.InOutBack)).OnComplete(() => action?.Invoke());
        }
    }
}