using System.Collections.Generic;
using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.Stars
{
    [RequireComponent(typeof(StarsCounter))]

    public class StarRatingCalculator : MonoBehaviour
    {
        [SerializeField] private float _starSpacing = 10;
        [SerializeField] private Image _starPrefab;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _positionMobile;

        private TextMeshProUGUI _linePrefab;
        private StarsCounter _starCounter;
        private int _stepSeconds = 1;
        private int _heightDivider;
        private Dictionary<int, List<int>> _starsToTimes;
        private ZoomChanger _zoomChanger;

        private void Awake()
        {
            _stepSeconds = 1;
            _heightDivider = 2;
            _starCounter = GetComponent<StarsCounter>();
            _linePrefab = GetComponentInChildren<TextMeshProUGUI>();
            _starsToTimes = new Dictionary<int, List<int>>();
            _zoomChanger = new ZoomChanger();
        }

        private void Start()
        {
            PrintAllStarRatings();
            CreateUI();

            if (_zoomChanger.IsMobileWithTallScreen())
            {
                _rectTransform.anchoredPosition = new Vector2(_positionMobile, _rectTransform.anchoredPosition.y);
            }
        }

        private void PrintAllStarRatings()
        {
            FillDictionary();
        }

        private void FillDictionary()
        {
            _starsToTimes.Clear();

            for (int time = 1; time <= _starCounter.MaxTimeSeconds; time += _stepSeconds)
            {
                int stars = _starCounter.GetCountStars(time);

                if (_starsToTimes.ContainsKey(stars) == false)
                    _starsToTimes[stars] = new List<int>();

                _starsToTimes[stars].Add(time);
            }
        }

        private void CreateUI()
        {
            ClearContainer();

            RectTransform prefabRect = _linePrefab.rectTransform;
            Vector2 startPos = prefabRect.anchoredPosition;

            int index = 0;

            foreach (var pair in _starsToTimes)
            {
                TextMeshProUGUI line = Instantiate(_linePrefab, transform);
                line.text = FormatLine(pair.Value[0], pair.Value[^1]);
                line.enabled = true;

                RectTransform rect = line.rectTransform;
                rect.anchoredPosition =
                startPos - new Vector2(0f, index * GetRowSpacing(prefabRect.rect.height));

                _starPrefab.transform.position = line.transform.position;

                CreateStars(pair.Key, rect.anchoredPosition);

                index++;
            }
        }

        private void CreateStars(int stars, Vector2 anchoredPosition)
        {
            for (int s = 0; s < stars; s++)
            {
                Image star = Instantiate(_starPrefab, transform);

                RectTransform starRect = star.rectTransform;
                starRect.anchoredPosition = anchoredPosition + new Vector2(s * _starSpacing, 0f);
                star.enabled = true;
            }
        }

        private float GetRowSpacing(float height)
        {
            return height /= _heightDivider;
        }

        private string FormatLine(int from, int to)
        {
            return $"{from} – {to} {_text.text}";
        }

        private void ClearContainer()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}