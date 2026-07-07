using System;
using PuzzleResources.LockMechanics;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle
{
    public class StateTutorial : MonoBehaviour
    {
        [SerializeField] private TextSwitcher _textSwitcher;

        private Key _key;
        private Lock _lock;
        private HandMover _handMover;
        private TouchVisualizer _visualizer;
        private bool _isClick;

        public event Action Completed;

        private void OnDisable()
        {
            if (_key == null)
                return;

            if (_lock == null)
                return;

            _key.Shift -= OnMovePointer;
            _key.Selected -= OnMovePointerClick;
            _lock.Unblocking -= OnComplete;
        }

        public void Initialization(HandMover handMover,
        TouchVisualizer touchVisualizer,
        Key key,
        Lock @lock)
        {
            _key = key;
            _lock = @lock;
            _handMover = handMover;
            _visualizer = touchVisualizer;

            SubscribeEvents();

            Begin();
        }

        private void SubscribeEvents()
        {
            _key.Shift += OnMovePointer;
            _key.Selected += OnMovePointerClick;
            _lock.Unblocking += OnComplete;
        }

        private void Begin()
        {
            _handMover.EnableLoopingAnimationZ();
        }

        private void SetPositionsEquipment(Vector3 position)
        {
            _handMover.SetPosition(position);
            _visualizer.SetPosition(position);
        }

        private void OnMovePointerClick()
        {
            SetPositionsEquipment(_lock.transform.position);
        }

        private void OnMovePointer()
        {
            if (_isClick == false)
            {
                _isClick = true;

                SetPositionsEquipment(_key.transform.position);

                _visualizer.gameObject.SetActive(true);

                _handMover.Stop();
                _handMover.EnableScaleAnimation();
            }
        }

        private void OnComplete()
        {
            _visualizer.gameObject.SetActive(false);
            _handMover.gameObject.SetActive(false);

            Completed?.Invoke();

            _textSwitcher.gameObject.SetActive(true);
            _textSwitcher.TurnOffDesiredOne(false);
        }
    }
}