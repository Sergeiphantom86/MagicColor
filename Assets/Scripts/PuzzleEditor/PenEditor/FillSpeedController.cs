using System;

namespace PuzzleEditor.PenEditor
{
    public class FillSpeedController
    {
        private bool _isAccelerated;

        public void Increase(float remainingTime, Action<float> notify, Action applySpeed)
        {
            if (_isAccelerated)
            return;

            _isAccelerated = true;

            notify?.Invoke(remainingTime);

            applySpeed?.Invoke();
        }
    }
}