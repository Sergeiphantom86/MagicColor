using Menu.Tutorials.TutorialPuzzle;
using PuzzleResources.LockMechanics;
using UnityEngine;

namespace PuzzleResources.Walls
{
    public class LockInteractionHandler
    {
        private Lock _lock;
        private Messager _hintKey;
        private bool _initialized;

        public void SetHint(Messager hintKey) => _hintKey = hintKey;

        public void Set(Collider other)
        {
            if (other.TryGetComponent(out Lock @lock))
            {
                if (_initialized == false)
                {
                    _initialized = true;
                    _lock = @lock;
                }
            }
        }

        public void Unblock()
        {
            if (_lock != null)
            {
                _lock.Unblock();
            }
        }
    }
}