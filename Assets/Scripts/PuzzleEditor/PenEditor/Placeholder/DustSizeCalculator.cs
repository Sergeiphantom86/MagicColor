using UnityEngine;

namespace PuzzleEditor.PenEditor.Placeholder
{
    public class DustSizeCalculator : MonoBehaviour
    {
        [Header("��������� ������������")]
        [SerializeField]
        private float _minSize = 0.1f;

        [SerializeField]
        private float _maxSize = 1f;

        [Header("�������")]
        [SerializeField]
        private float _calculatedSize;

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
                Debug.LogWarning(
                    $"�������� ������������ �� ����� ���� �������������. ��������: {amount}. ����������� �������� 0."
                );
            }
        }

        private void CheckUpperLimit(int upperLimit)
        {
            if (upperLimit <= 0)
            {
                Debug.LogError(
                    $"������������ �������� ������������ ������ ���� �������������. ��������: {upperLimit}. ����������� �������� �� ���������: 500."
                );
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
                Debug.LogWarning(
                    $"����������� ������ �� ����� ���� �������������. ��������: {_minSize}. ����������� �������� 0."
                );
                _minSize = 0f;
            }
        }

        private void VerifyBoundsConsistency()
        {
            if (_maxSize < _minSize)
            {
                Debug.LogWarning(
                    $"������������ ������ �� ����� ���� ������ ������������. ������������: {_maxSize}, �����������: {_minSize}. ����������� �������� �� ���������."
                );
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