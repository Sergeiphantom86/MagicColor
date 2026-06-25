using System.Linq;
using Game.SaveEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.RouletteEditor
{
    public class Currency : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMeshPro;

        [SerializeField]
        private Image _image;

        private WeightCalculator _weightCalculator;
        private IProgressSaver _progressSaver;
        private int _value;
        private int _weight;
        private int _divider;
        private int _indexInRoulette;
        private float _sectorSize;
        private float _quantityDegrees;

        public int Weight => _weight;

        public int Value => _value;

        public Image Icon => _image;

        public int Winn => _progressSaver.Saves.Reward;

        private void Awake()
        {
            _divider = 2;
            _indexInRoulette = -1;
            _quantityDegrees = 360;

            _weightCalculator = new WeightCalculator();
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            _image = GetComponentInChildren<Image>();
            _progressSaver = new ProgressSaver();

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

        public void Initialize(int index, int totalItems)
        {
            _indexInRoulette = index;
            _sectorSize = _quantityDegrees / totalItems;
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
            return _sectorSize / _divider;
        }

        private string GetCleanNumericString(string textMeshPro)
        {
            return new string(textMeshPro.Where(c => char.IsDigit(c) || c == '-').ToArray());
        }
    }
}