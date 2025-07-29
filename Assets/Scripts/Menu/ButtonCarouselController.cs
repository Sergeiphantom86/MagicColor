using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ButtonKeeper))]
public class ButtonCarouselController : MonoBehaviour
{
    [SerializeField] private int _defaultIndex = 0;
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _easeType = Ease.OutBack;


    private RectTransform[] _buttons;
    private Vector2[] _originalPositions;
    private int _currentIndex;
    private float _screenWidth;
    private ButtonKeeper _buttonKeeper;
    private RectTransform _canvasRect;
    private bool _isInitialized;
    private float _screenWidthMultiplier;

    public RectTransform CurrentButton => _buttons[_currentIndex];
    public Vector2 CurrentButtonOriginalPosition => _originalPositions[_currentIndex];

    private void Awake()
    {
        _screenWidthMultiplier = 1.1f;
        _buttonKeeper = GetComponent<ButtonKeeper>();
        _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        InitializeSystem();
    }

    private void InitializeSystem()
    {
        ValidateButtons();
        InitializeArrays();
        CacheOriginalPositions();
        CalculateScreenWidth();
        SetupInitialPositions();
        _isInitialized = true;
    }

    private void ValidateButtons()
    {
        if (_buttonKeeper == null || _buttonKeeper.Buttons.Length == 0)
            Debug.LogError("Button components are not assigned!");
    }

    private void InitializeArrays()
    {
        _buttons = new RectTransform[_buttonKeeper.Buttons.Length];
        _originalPositions = new Vector2[_buttonKeeper.Buttons.Length];
    }

    private void CacheOriginalPositions()
    {
        for (int i = 0; i < _buttonKeeper.Buttons.Length; i++)
        {
            if (_buttonKeeper.Buttons[i] == null)
            {
                Debug.LogError($"Button at index {i} is null!");
                continue;
            }

            _buttons[i] = _buttonKeeper.Buttons[i].GetComponent<RectTransform>();
            _originalPositions[i] = _buttons[i].anchoredPosition;
        }
    }

    private void CalculateScreenWidth()
    {
        if (_canvasRect == null)
        {
            Debug.LogError("Canvas not found in parents!");
            _screenWidth = Screen.width;
            return;
        }

        _screenWidth = _canvasRect.rect.width;

        _screenWidth *= _screenWidthMultiplier;
    }

    private void SetupInitialPositions()
    {
        _currentIndex = Mathf.Clamp(_defaultIndex, 0, _buttons.Length - 1);

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] == null) continue;

            float targetX = i == _currentIndex
                ? _originalPositions[i].x
                : (i < _currentIndex ? -_screenWidth : _screenWidth);

            UpdateButtonPosition(_buttons[i], targetX);
        }
    }

    public void ShowRelative(int direction)
    {
        if (_isInitialized == false) return;

        int newIndex = _currentIndex + direction;

        if (newIndex >= 0 && newIndex < _buttons.Length)
        {
            ShowButton(newIndex);
        }
    }

    public void UpdateButtonPosition(RectTransform button, float xPosition)
    {
        if (button == null) return;

        Vector2 pos = button.anchoredPosition;
        pos.x = xPosition;
        button.anchoredPosition = pos;
    }

    private void ShowButton(int index)
    {
        if (index < 0 || index >= _buttons.Length) return;

        if (index == _currentIndex) return;

        if (_buttons[index] == null) return;

        int direction = index > _currentIndex ? -1 : 1;

        AnimateTransition(_currentIndex, direction);

        SetupNewButton(index, -direction);

        _currentIndex = index;
    }

    private void AnimateTransition(int index, int direction)
    {
        if (_buttons[index] == null) return;

        _buttons[index].DOAnchorPosX(direction * _screenWidth, _animationDuration)
            .SetEase(_easeType).SetEase(_easeType);
    }

    private void SetupNewButton(int index, int direction)
    {
        if (_buttons[index] == null) return;

        UpdateButtonPosition(_buttons[index], direction * _screenWidth);
        _buttons[index].DOAnchorPosX(_originalPositions[index].x, _animationDuration)
            .SetEase(_easeType);
    }
}