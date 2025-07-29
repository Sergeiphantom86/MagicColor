using System.Linq;
using UnityEngine;
using YG;
using YG.Utils.LB;

[RequireComponent(typeof(StarsController))]
public class LeaderboardStars : MonoBehaviour
{
    private string _leaderboardName = "PuzzleDogBestTime";
    private StarsController _starsController;
    private int _lastSavedScore;
    private int _maxStars = 5;
    private int _minStars = 1;
    private int _maxTimeSeconds = 60;

    private void Awake()
    {
        _starsController = GetComponent<StarsController>();
    }

    private void OnEnable() => YG2.onGetLeaderboard += OnLeaderboardLoaded;
    private void OnDisable() => YG2.onGetLeaderboard -= OnLeaderboardLoaded;

    private void Start()
    {
        _lastSavedScore = PlayerPrefs.GetInt("LastLevelTime", 0);
        Debug.Log($"Loaded saved time: {_lastSavedScore}s");
        LoadLeaderboard();
    }

    public void LoadLeaderboard() => YG2.GetLeaderboard(_leaderboardName);

    public void SavePlayerTime(float timeInSeconds)
    {
        int seconds = Mathf.RoundToInt(timeInSeconds);

        _lastSavedScore = seconds;
        PlayerPrefs.SetInt("LastLevelTime", seconds);

        YG2.SetLBTimeConvert(_leaderboardName, timeInSeconds);
    }

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

        float progress = 1f - Mathf.Clamp01((float)timeInSeconds / _maxTimeSeconds);
        int stars = Mathf.RoundToInt(_minStars + progress * (_maxStars - _minStars));

        return Mathf.Clamp(stars, _minStars, _maxStars);
    }
}