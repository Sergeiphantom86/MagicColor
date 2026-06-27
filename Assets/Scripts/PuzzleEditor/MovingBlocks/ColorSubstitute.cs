using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    public class ColorSubstitute : ColorableObject
    {
        [Header("��������� �����")]
        [SerializeField] private bool _changeOnAwake = true;
        [SerializeField] private bool _useSaturationRange = false;
        [SerializeField][Range(0f, 1f)] private float _minSaturation = 0.5f;
        [SerializeField][Range(0f, 1f)] private float _maxSaturation = 1f;
        [SerializeField] private bool _useValueRange = false;
        [SerializeField][Range(0f, 1f)] private float _minValue = 0.5f;
        [SerializeField][Range(0f, 1f)] private float _maxValue = 1f;

        private void Awake()
        {
            InitializeComponents();

            if (_changeOnAwake)
            {
                ChangeToRandomColor();
            }
        }

        public void ChangeToRandomColor()
        {
            Color randomColor;

            if (_useSaturationRange || _useValueRange)
            {
                randomColor = Random.ColorHSV(
                    0f,
                    1f,
                    _useSaturationRange ? _minSaturation : 0f,
                    _useSaturationRange ? _maxSaturation : 1f,
                    _useValueRange ? _minValue : 0f,
                    _useValueRange ? _maxValue : 1f);
            }
            else
            {
                randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }

            SetColor(randomColor);
            InstallRepainted();
            AssignOriginal();
        }
    }
}