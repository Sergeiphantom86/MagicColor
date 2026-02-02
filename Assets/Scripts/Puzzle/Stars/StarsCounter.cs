using UnityEngine;

public class StarsCounter : MonoBehaviour
{
    private int _maxStars;
    private int _minStars;
    private int _maxTimeSeconds;

    public int MaxStars => _maxStars;
    public int MinStars => _minStars;
    public int MaxTimeSeconds => _maxTimeSeconds;

    private void Awake()
    {
        _maxStars = 5;
        _minStars = 1;
        _maxTimeSeconds = 60;
    }

    public float GetTimePerStar()
    {
        return (float)_maxTimeSeconds / (_maxStars - _minStars + 1);
    }

    public int CalculateStarsByAbsoluteTime(int timeInSeconds)
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