using Menu.Tutorials.TutorialPuzzle;
using PuzzleResources.LockMechanics;
using UnityEngine;

namespace PuzzleResources.Walls
{
    public class LockFeedbackService : MonoBehaviour, ILockFeedbackService
    {
        private Lock _lock;
        private Messager _hint;

        public void InitializeComponents(Lock @lock, Messager hint)
        {
            SetLock(@lock);
            SetKey(hint);
        }

        public void Play()
        {
            if (_lock != null)
            {
                _lock.ShakeUp();
            }

            if (_hint != null)
            {
                _hint.TurnOn();
            }
        }

        private void SetLock(Lock @lock)
        {
            if (@lock == null)
            {
                Debug.LogError("Lock == null");
                return;
            }

            _lock = @lock;
        }

        private void SetKey(Messager hint)
        {
            if (hint == null)
            {
                Debug.LogError("HintKey == null");
                return;
            }

            _hint = hint;
        }
    }
}