using UnityEngine;

namespace Menu
{
    public class ButtonCarouselController : MonoBehaviour, ICarousel
    {
        [SerializeField] private int _defaultIndex;
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _centerScale;
        [SerializeField] private float _sideScale;
        [SerializeField] private float _centerAlpha;
        [SerializeField] private float _sideAlpha;
        [SerializeField] private float _distanceFromCenter;

        private int _currentIndex;
        private bool _isInitialized;
        private CarouselData _data;
        private CarouselLayoutCalculator _layout;
        private CarouselAnimator _animator;

        public int CurrentIndex => _currentIndex;

        public float ScrollDuration => _animationDuration;

        private void Awake()
        {
            InitializeSystem();
        }

        public void ShowRelative(int direction)
        {
            if (_isInitialized == false)
                return;

            ScrollToButton(_currentIndex + direction);
        }

        public void ScrollToButton(int targetIndex)
        {
            if (_isInitialized == false)
                return;

            if (targetIndex < 0 || targetIndex >= _data.Buttons.Length)
                return;

            if (targetIndex == _currentIndex)
                return;

            _currentIndex = targetIndex;

            UpdateAllButtons(false);
        }

        private void InitializeSystem()
        {
            var keeper = GetComponent<ButtonKeeper>();

            if (keeper == null || keeper.Buttons.Length == 0)
            {
                Debug.LogError("ButtonKeeper missing");
                return;
            }

            _data = new CarouselData(keeper);

            _currentIndex = Mathf.Clamp(_defaultIndex, 0, _data.Buttons.Length - 1);

            _layout = new CarouselLayoutCalculator(
                _data.Buttons,
                _data.OriginalPositions,
                _centerScale,
                _sideScale,
                _distanceFromCenter);

            _animator = new CarouselAnimator(_animationDuration);

            UpdateAllButtons(true);

            _isInitialized = true;
        }

        private void UpdateAllButtons(bool instant)
        {
            for (int i = 0; i < _data.Buttons.Length; i++)
            {
                UpdateButton(i, instant);
            }
        }

        private void UpdateButton(int index, bool instant)
        {
            ButtonVisualState state = CalculateVisualState(index);

            ApplyVisualState(index, state, instant);
        }

        private void ApplyVisualState(int index, ButtonVisualState state, bool instant)
        {
            if (instant)
            {
                _animator.ApplyImmediate(
                    _data.Buttons[index],
                    _data.CanvasGroups[index],
                    state.PositionX,
                    state.Scale,
                    state.Alpha);

                return;
            }

            _animator.ApplyAnimated(
                _data.Buttons[index],
                _data.CanvasGroups[index],
                state.PositionX,
                state.Scale,
                state.Alpha);
        }

        private ButtonVisualState CalculateVisualState(int index)
        {
            bool isCenter = index == _currentIndex;

            return new ButtonVisualState
            {
                PositionX = _layout.GetTargetPositionX(index, _currentIndex),
                Scale = isCenter ? _centerScale : _sideScale,
                Alpha = isCenter ? _centerAlpha : _sideAlpha,
            };
        }

        private struct ButtonVisualState
        {
            public float PositionX;
            public float Scale;
            public float Alpha;
        }
    }
}