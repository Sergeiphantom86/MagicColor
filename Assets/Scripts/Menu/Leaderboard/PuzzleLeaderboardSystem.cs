using System.Collections.Generic;
using UnityEngine;
using YG;
using static YG.YG2;

public class PuzzleLeaderboardSystem : MonoBehaviour
{
    public List<PuzzleLeaderboard> _leaderboards;
    public LeaderboardYG _leaderboardPrefab;
    public Transform _leaderboardParent;

    private Dictionary<Sprite, LeaderboardYG> _activeLeaderboards = new();

    void Start()
    {
        foreach (var lb in _leaderboards)
        {
            CreateLeaderboardForPuzzle(lb);
        }
    }

    void CreateLeaderboardForPuzzle(PuzzleLeaderboard config)
    {
        LeaderboardYG leaderboardYG = Instantiate(_leaderboardPrefab, _leaderboardParent);
        leaderboardYG.nameLB = config.technicalName;
        leaderboardYG.gameObject.SetActive(false);

        _activeLeaderboards.Add(config.puzzleSprite, leaderboardYG);
    }

    public void ShowLeaderboardForPuzzle(Sprite selectedPuzzle)
    {
        if (_activeLeaderboards.TryGetValue(selectedPuzzle, out LeaderboardYG leaderboardYG))
        {
            foreach (var board in _activeLeaderboards.Values)
            {
                board.gameObject.SetActive(false);
            }

            leaderboardYG.gameObject.SetActive(true);
            leaderboardYG.UpdateLB();
        }
    }

    public float GetBestTimeForPuzzle(Sprite puzzle)
    {
        var saves = YG2.saves;
        string puzzleKey = puzzle.name;

        if (saves.PuzzleBestTimes != null &&
            saves.PuzzleBestTimes.TryGetValue(puzzleKey, out float bestTime))
        {
            return bestTime;
        }

        return float.MaxValue;
    }
}