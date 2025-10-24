using UnityEngine;
using YG;
using YG.Utils.LB;

[RequireComponent(typeof(StarsController))]
public class LeaderboardStars : MonoBehaviour
{
    private string _leaderboardName;
    private int _lastSavedScore;
    private int _maxStars = 5;
    private int _minStars = 1;
    private int _maxTimeSeconds = 60;
    private StarsController _starsController;

    private void Awake()
    {
        _maxStars = 5;
        _minStars = 1;
        _maxTimeSeconds = 60;
        _starsController = GetComponent<StarsController>();
    }

    private void OnEnable() => 
        YG2.onGetLeaderboard += OnLeaderboardLoaded;
    private void OnDisable() => 
        YG2.onGetLeaderboard -= OnLeaderboardLoaded;

    private void Start()
    {
        LoadLeaderboard();
    }

    public void SavePlayerTime(float timeInSeconds, string leaderboardName)
    {
        int seconds = Mathf.RoundToInt(timeInSeconds);
        _leaderboardName = leaderboardName;
        _lastSavedScore = seconds;

        YG2.SetLBTimeConvert(_leaderboardName, timeInSeconds);
    }

    private void LoadLeaderboard() => YG2.GetLeaderboard(_leaderboardName);

    private void OnLeaderboardLoaded(LBData data)
    {
        if (data.technoName != _leaderboardName) return;

        _starsController.ShowStars(CalculateStarsByAbsoluteTime(_lastSavedScore));
    }

    private int CalculateStarsByAbsoluteTime(int timeInSeconds)
    {
        if (timeInSeconds <= 0)
        {
            Debug.LogError("Invalid time! Using fallback");
            return _minStars;
        }

        if (timeInSeconds > _maxTimeSeconds) return _minStars;

        return Mathf.Clamp(GetStars(timeInSeconds), _minStars, _maxStars);
    }

    private float GetProgress(int timeInSeconds)
    {
        return 1f - Mathf.Clamp01((float)timeInSeconds / _maxTimeSeconds);
    }

    private int GetStars(int timeInSeconds)
    {
        return Mathf.RoundToInt(_minStars + GetProgress(timeInSeconds) * (_maxStars - _minStars));
    }
}