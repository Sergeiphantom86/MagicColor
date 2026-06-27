using UnityEngine;

namespace PuzzleEditor.Stars
{
    public class StarsCounter : MonoBehaviour
    {
        private int _baseMaxStars;
        private int _currentMaxStars;
        private int _minStars;
        private int _maxTimeSeconds;

        public int MinStars => _minStars;

        public int MaxStars => _currentMaxStars;

        public int MaxTimeSeconds => _maxTimeSeconds;

        private void Awake()
        {
            _baseMaxStars = 5;
            _currentMaxStars = _baseMaxStars;
            _minStars = 1;
            _maxTimeSeconds = 360;
        }

        public int DisableOneStar()
        {
            _currentMaxStars = Mathf.Max(_minStars, _baseMaxStars - 1);

            return _currentMaxStars;
        }

        public int EnableOneStar()
        {
            _currentMaxStars = _baseMaxStars;

            return _currentMaxStars;
        }

        public int GetCountStars(int timeInSeconds)
        {
            if (timeInSeconds > _maxTimeSeconds)
                return _minStars;

            return Mathf.Clamp(GetStars(timeInSeconds, _maxTimeSeconds), _minStars, _currentMaxStars);
        }

        private int GetStars(int timeInSeconds, int maxTimeSeconds)
        {
            return Mathf.RoundToInt(_minStars + GetProgress(
                timeInSeconds,
                maxTimeSeconds) * (_currentMaxStars - _minStars));
        }

        private float GetProgress(int timeInSeconds, int maxTimeSeconds)
        {
            return 1f - Mathf.Clamp01((float)timeInSeconds / maxTimeSeconds);
        }
    }
}