using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Currency : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Image _image;

    private WeightCalculator _weightCalculator;
    private int _value;
    private int _weight;
    private int _indexInRoulette;
    private float _sectorSize;

    public int Weight => _weight;

    public int Value => _value;

    public Image Icon => _image;

    public int Winn => YG2.saves.Reward;

    private void Awake()
    {
        _indexInRoulette = -1;
        _weightCalculator = new WeightCalculator();
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponentInChildren<Image>();

        if (_textMeshPro == null)
        {
            Debug.LogWarning($"TextMeshPro component not found for {name}", this);
            return;
        }

        if (_image == null)
        {
            Debug.LogWarning($"Image component not found for {name}", this);
            return;
        }

        SetValue(GetCleanNumericString(_textMeshPro.text));

        _weight = _weightCalculator.GetWeight(_value);
    }

    public void Initialize( int index, int totalItems)
    {
        _indexInRoulette = index;
        _sectorSize = 360f / totalItems;
    }

    public float GetAngle()
    {
        return _indexInRoulette * _sectorSize + GetSectorCenter();
    }

    protected void SetValue(string value)
    {
        if (int.TryParse(value, out int parsedValue))
        {
            _value = parsedValue;
        }
    }

    private float GetSectorCenter()
    {
        return _sectorSize / 2f;
    }

    private string GetCleanNumericString(string textMeshPro)
    {
        return new string(textMeshPro
            .Where(c => char.IsDigit(c) || c == '-')
            .ToArray());
    }
}