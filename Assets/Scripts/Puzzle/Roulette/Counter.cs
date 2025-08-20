using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ButtonController))]
public class Counter : MonoBehaviour
{
    [SerializeField] private RewardAdForSpins _rewardAdForSpins;

    private int _currentCount;
    private int _displayedCount;
    private int _initialCount;
    private float _animationDuration;
    private Tween _countTween;
    private TextMeshProUGUI _counterText;
    private ButtonController _buttonController;

    public bool HasAttempts => _currentCount > 0;

    private void OnEnable()
    {
        _rewardAdForSpins.OnSpinsAdded += AddSpin;
    }

    private void OnDisable()
    {
        _rewardAdForSpins.OnSpinsAdded -= AddSpin;
    }

    private void Awake()
    {
        _initialCount = 1;
        _animationDuration = 0.5f;

        _currentCount = _initialCount;
        _displayedCount = _initialCount;

        _buttonController = GetComponent<ButtonController>();
        _counterText = GetComponentInChildren<TextMeshProUGUI>();

        if (_counterText == null)
        {
            Debug.LogError("TextMeshProUGUI нет у детей!!!");
            return;
        }

        if (_buttonController != null)
        {
            _buttonController.Initialize(
                globalInteractableCondition: () => true,
                onClickAction: DecreaseCount
            );

            _buttonController.UpdateState();
        }
        else
        {
            Debug.LogError("ButtonController не назначен!", this);
        }
    }

    private void Start()
    {
        UpdateText();
    }

    private void AddSpin()
    {
        _currentCount++;
        AnimateCounterChange();

        if (_buttonController != null)
            _buttonController.UpdateState();
    }

    public void DecreaseCount()
    {
        if (_currentCount <= 0) return;

        _currentCount--;
        AnimateCounterChange();
        _buttonController.UpdateState();
    }

    private void AnimateCounterChange()
    {
        _countTween?.Kill();

        _countTween = DOTween.To(() => _displayedCount,
            currentValue => 
            {
                _displayedCount = currentValue;
                UpdateText();
            },
            _currentCount,
            _animationDuration
        ).SetEase(Ease.OutQuad);
    }

    private void UpdateText()
    {
        _counterText.text = _displayedCount.ToString();
    }

    private void OnDestroy()
    {
        _countTween?.Kill();
    }
}