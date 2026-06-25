using System;

namespace PuzzleEditor.PenEditor
{
    public class PuzzleProgressTracker
    {
        private int _remaining;

        public event Action PuzzleCompleted;

        public void Init(int totalPixels)
        {
            _remaining = totalPixels;
        }

        public void OnFragmentActivated()
        {
            _remaining--;

            if (_remaining <= 0)
            {
                PuzzleCompleted?.Invoke();
            }
        }
    }
}