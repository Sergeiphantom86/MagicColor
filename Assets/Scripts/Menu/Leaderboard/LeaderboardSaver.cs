using System.Collections.Generic;
using UnityEngine;
using YG;
using static YG.YG2;

public class LeaderboardSaver : MonoBehaviour
{
    private Dictionary<Sprite, LeaderboardYG> _activeLeaderboards;

    private void Awake()
    {
        _activeLeaderboards = new();
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