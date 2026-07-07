using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace PuzzleResources.MinigamesRoulette
{
    [RequireComponent(typeof(ButtonController))]

    public class RouletteCounter : MonoBehaviour
    {
        [SerializeField] private RewardAdForSpins _rewardAdForSpins;

        private int _currentCount;
        private int _displayedCount;
        private int _initialCount;
        private float _animationDuration;
        private Tween _countTween;
        private TextMeshProUGUI _counterText;
        private ButtonController _buttonController;
        private Image _image;

        public bool HasAttempts => _currentCount > 0;

        private void OnEnable()
        {
            _rewardAdForSpins.SpinsAdded += OnAddSpin;
        }

        private void OnDisable()
        {
            _rewardAdForSpins.SpinsAdded -= OnAddSpin;
        }

        private void Awake()
        {
            _animationDuration = 0.5f;

            _buttonController = GetComponent<ButtonController>();
            _counterText = GetComponentInChildren<TextMeshProUGUI>();
            _image = GetComponent<Image>();

            if (_counterText == null)
            {
                Debug.LogError("TextMeshProUGUI reference is missing!", this);
                return;
            }

            if (_buttonController != null)
            {
                _buttonController.Initialize(() =>
                true, onClickAction: DecreaseCount);

                _buttonController.UpdateState();
            }
            else
            {
                Debug.LogError("ButtonController is not assigned!", this);
            }

            SetCoutSpins();

            _rewardAdForSpins.gameObject.SetActive(false);
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            YG2.saves.Spins = _currentCount;
            _countTween?.Kill();
        }

        private void SetCoutSpins()
        {
            _initialCount = YG2.saves.Spins;

            _initialCount++;

            _currentCount = _initialCount;
            _displayedCount = _initialCount;
        }

        public void DecreaseCount()
        {
            if (_currentCount <= 0)
                return;

            _currentCount--;
            AnimateCounterChange();
            _buttonController.UpdateState();

            if (_currentCount == 0)
            {
                SwitchVisibility(false);

                _rewardAdForSpins.gameObject.SetActive(true);
            }
        }

        private void SwitchVisibility(bool isOn)
        {
            _counterText.enabled = isOn;
            _image.enabled = isOn;
        }

        private void AnimateCounterChange()
        {
            _countTween?.Kill();

            _countTween = DOTween.To(() => _displayedCount, currentValue =>
            {
                _displayedCount = currentValue;

                UpdateText();
            },
                _currentCount, _animationDuration)
                .SetEase(Ease.OutQuad);
        }

        private void UpdateText()
        {
            _counterText.text = _displayedCount.ToString();
        }

        private void OnAddSpin()
        {
            SwitchVisibility(true);

            if (_buttonController != null)
            {
                _buttonController.UpdateState();
            }

            _currentCount++;

            AnimateCounterChange();
        }
    }
}