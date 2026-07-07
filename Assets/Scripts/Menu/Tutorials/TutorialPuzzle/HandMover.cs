using System;
using DG.Tweening;
using PuzzleEditor.MovingBlocks;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle
{
    public class HandMover : MonoBehaviour
    {
        [SerializeField] private Pivot _pivot;

        private Vector3 _startScale;
        private Vector3 _targetScale;
        private Sequence _sequence;
        private float _distanceZ;
        private float _duration;
        private float _overshoot;
        private int _scaleMultiplier;

        public event Action Destroyed;

        public Pivot Pivot => _pivot;

        private void Awake()
        {
            _distanceZ = 2;
            _overshoot = 5;
            _duration = 0.7f;
            _scaleMultiplier = 20;

            _targetScale = Vector3.one * _scaleMultiplier;
            _startScale = transform.localScale;
        }

        private void Start()
        {
            SetPosition(transform.position);
        }

        private void OnDisable()
        {
            Stop();
        }

        public void SetPosition(Vector3 position)
        {
            position.y += 0.5f;

            transform.position = position;
        }

        public void EnableScaleAnimation()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _sequence.Append(transform.DOScale(_targetScale, _duration).SetEase(Ease.OutBack, _overshoot))
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);
        }

        public void EnableMoveAnimationZ()
        {
            GetAnimationSequence(0, _distanceZ).SetLoops(-1, LoopType.Restart);
        }

        public void EnableLoopingAnimationZ()
        {
            GetAnimationSequence(0, _distanceZ).SetLoops(-1, LoopType.Restart);
        }

        public Sequence GetAnimationSequence(float distanceX = 0, float distance = 0)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _sequence.AppendInterval(_duration);
            _sequence.Join(transform.DOMove(GetTargetPosition(distanceX, distance), 1f));

            _sequence.SetUpdate(true);

            return _sequence;
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }

        public void TurnOn()
        {
            gameObject.SetActive(true);
        }

        public void Stop()
        {
            _sequence?.Kill();

            transform.localScale = Vector3.one * _startScale.x;
        }

        public void Destroy()
        {
            Destroyed?.Invoke();
            TurnOff();
        }

        private Vector3 GetTargetPosition(float distanceX = 0, float distance = 0)
        {
            Vector3 position = transform.position;

            position.x -= distanceX;
            position.z -= distance;

            return position;
        }
    }
}