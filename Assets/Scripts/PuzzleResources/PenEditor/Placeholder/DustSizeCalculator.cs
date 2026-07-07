using UnityEngine;

namespace PuzzleResources.PenEditor.Placeholder
{
    public class DustSizeCalculator : MonoBehaviour
    {
        [SerializeField] private float _minSize = 0.1f;
        [SerializeField] private float _maxSize = 1f;
        [SerializeField] private float _calculatedSize;

        public float CalculateSize(int quantity, int maxDustValue)
        {
            CheckInputParameters(quantity, maxDustValue);
            VerifyRangeSettings();

            float percentage = GetPercentageValue(quantity, maxDustValue);
            _calculatedSize = ComputeInterpolatedValue(percentage);

            return _calculatedSize;
        }

        private void CheckInputParameters(int quantity, int maxDustValue)
        {
            VerifyAmount(quantity);
            CheckUpperLimit(maxDustValue);
        }

        private void VerifyAmount(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"Negative amount {amount}, clamp to 0", this);
            }
        }

        private void CheckUpperLimit(int upperLimit)
        {
            if (upperLimit <= 0)
            {
                Debug.LogWarning($"UpperLimit {upperLimit} <=0, using 500", this);
            }
        }

        private void VerifyRangeSettings()
        {
            CheckLowerBound();
            VerifyBoundsConsistency();
        }

        private void CheckLowerBound()
        {
            if (_minSize < 0f)
            {
                Debug.LogWarning($"CheckLowerBound: minimum size cannot be negative. Value: {_minSize}. Setting to 0.");
                _minSize = 0f;
            }
        }

        private void VerifyBoundsConsistency()
        {
            if (_maxSize < _minSize)
            {
                Debug.LogWarning($"Max {_maxSize} < min {_minSize}, reset to (0.1, 1)", this);
                _minSize = 0.1f;
                _maxSize = 1f;
            }
        }

        private float GetPercentageValue(int currentAmount, int maximumAmount)
        {
            float clampedValue = Mathf.Clamp(currentAmount, 0f, maximumAmount);
            return clampedValue / maximumAmount;
        }

        private float ComputeInterpolatedValue(float interpolationFactor)
        {
            return Mathf.Lerp(_minSize, _maxSize, interpolationFactor);
        }
    }
}