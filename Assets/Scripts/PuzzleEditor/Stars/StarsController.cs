using DG.Tweening;
using UnityEngine;
namespace PuzzleEditor.Stars
{

public class StarsController : MonoBehaviour
{
    private float _initialDelay;
    private float _delayBetweenStars;
    private int _lastSavedScore;
    private StarIndicator[] _stars;
    private Sequence _animationSequence;
    private StarsCounter _starCounter;

    private void Awake()
    {
        _initialDelay = 0.5f;
        _delayBetweenStars = 0.3f;
        _stars = GetComponentsInChildren<StarIndicator>();
        _starCounter = GetComponent<StarsCounter>();
        SetActive(false);
    }

    private void Start()
    {
        ShowWithAnimation(_starCounter.GetCountStars(_lastSavedScore));
    }

    private void OnEnable()
    {
        ResetAll();
    }

    public void SavePlayerTime(float timeInSeconds)
    {
        if (timeInSeconds <= 0)
        {
            Debug.LogError($"Invalid time! Using fallback {timeInSeconds} {this}");
            return;
        }

        _lastSavedScore = Mathf.RoundToInt(timeInSeconds);
    }

    public void ShowWithAnimation(int activeCount)
    {
        _animationSequence?.Kill();

        activeCount = Mathf.Clamp(activeCount, 0, _stars.Length);

        _animationSequence = DOTween.Sequence();
        _animationSequence.AppendInterval(_initialDelay);

        for (int i = 0; i < activeCount; i++)
        {
            int index = i;
            _animationSequence.InsertCallback(_initialDelay + GetDelay(i), () =>
                _stars[index].TurnOn());
        }
    }

    private float GetDelay(int index)
    {
        return index * _delayBetweenStars;
    }

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    private void ResetAll()
    {
        foreach (var star in _stars)
        {
            star.SetInactive();
        }
    }
}
}