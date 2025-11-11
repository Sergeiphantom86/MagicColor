using UnityEngine;

[RequireComponent(typeof(StarsController))]
public class StarCounter : MonoBehaviour
{
    private int _maxStars;
    private int _minStars;
    private int _lastSavedScore;
    private int _maxTimeSeconds;
    private StarsController _starsController;

    private void Awake()
    {
        _maxStars = 5;
        _minStars = 1;
        _maxTimeSeconds = 120;
        _starsController = GetComponent<StarsController>();
    }

    private void Start()
    {
        _starsController.ShowWithAnimation(CalculateStarsByAbsoluteTime(_lastSavedScore));
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

    private int CalculateStarsByAbsoluteTime(int timeInSeconds)
    {
        if (timeInSeconds > _maxTimeSeconds) return _minStars;

        return Mathf.Clamp(GetStars(timeInSeconds), _minStars, _maxStars);
    }

    private int GetStars(int timeInSeconds)
    {
        return Mathf.RoundToInt(_minStars + GetProgress(timeInSeconds) * (_maxStars - _minStars));
    }

    private float GetProgress(int timeInSeconds)
    {
        return 1f - Mathf.Clamp01((float)timeInSeconds / _maxTimeSeconds);
    }
}