using UnityEngine;

public class DustSizeCalculator : MonoBehaviour
{
    [Header("Настройки запыленности")]
    [SerializeField] private float _minSize = 0.1f;
    [SerializeField] private float _maxSize = 1f;

    [Header("Отладка")]
    [SerializeField] private float _calculatedSize;

    public float CalculateSize(int quantity, int maxDustValue)
    {
        if (quantity < 0)
        {
            Debug.LogWarning($"Значение запыленности не может быть отрицательным. Получено: {quantity}. Установлено значение 0.");
            quantity = 0;
        }

        if (maxDustValue <= 0)
        {
            Debug.LogError($"Максимальное значение запыленности должно быть положительным. Получено: {maxDustValue}. Установлено значение по умолчанию: 500.");
            maxDustValue = 500;
        }

        if (_minSize < 0f)
        {
            Debug.LogWarning($"Минимальный размер не может быть отрицательным. Получено: {_minSize}. Установлено значение 0.");
            _minSize = 0f;
        }

        if (_maxSize < _minSize)
        {
            Debug.LogWarning($"Максимальный размер не может быть меньше минимального. Максимальный: {_maxSize}, Минимальный: {_minSize}. Установлены значения по умолчанию.");
            _minSize = 0.1f;
            _maxSize = 1f;
        }

        float clampedDustValue = Mathf.Clamp(quantity, 0f, maxDustValue);

        float dustPercentage = clampedDustValue / maxDustValue;
        _calculatedSize = Mathf.Lerp(_minSize, _maxSize, dustPercentage);

        return _calculatedSize;
    }
}