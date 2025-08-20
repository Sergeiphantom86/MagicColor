using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Award : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    private WeightCalculator _weightCalculator;
    private GameSaveSystem  _saveSystem;
    private Image _image;
    private int _value;
    private int _weight;
    private int _indexInRoulette;
    private float _sectorSize;

    public int Weight => _weight;

    public int Value => _value;

    public Sprite Sprite => _image.sprite;

    public int Winn => _saveSystem.CurrentValue;

    private void Awake()
    {
        _weightCalculator = new WeightCalculator();
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>(true);
        _image = GetComponentInChildren<Image>(true);
        _saveSystem = FindObjectOfType<GameSaveSystem>();

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

        if (_saveSystem == null)
        {
            Debug.LogWarning($"GameSaveSystem component not found for {name}", this);
            return;
        }

        _indexInRoulette = -1;
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

    private float GetSectorCenter()
    {
        return _sectorSize / 2f;
    }

    protected void SetValue(string value)
    {
        if (int.TryParse(value, out int parsedValue))
        {
            _value = parsedValue;
        }
    }

    private string GetCleanNumericString(string textMeshPro)
    {
        return new string(textMeshPro
            .Where(c => char.IsDigit(c) || c == '-')
            .ToArray());
    }
}