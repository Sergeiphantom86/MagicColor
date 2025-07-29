using UnityEngine;
using YG;

public class AdvManager : MonoBehaviour
{
    public const string PUZZLE_UNLOCK_REWARD_ID = "unlock_next_puzzle";

    // Текущий пазл, который нужно разблокировать
    private string _puzzleToUnlock;

    private void OnEnable() => YG2.onRewardAdv += OnReward;
    private void OnDisable() => YG2.onRewardAdv -= OnReward;

    // Вызов рекламы после завершения пазла
    public void ShowPuzzleRewardAd(string puzzleToUnlock)
    {
        if (!YG2.nowRewardAdv && !YG2.nowAdsShow)
        {
            _puzzleToUnlock = puzzleToUnlock;
            YG2.RewardedAdvShow(PUZZLE_UNLOCK_REWARD_ID);
            Debug.Log($"Starting rewarded ad to unlock: {puzzleToUnlock}");
        }
    }

    // Обработка награды - сразу разблокируем пазл
    private void OnReward(string rewardId)
    {
        if (rewardId == PUZZLE_UNLOCK_REWARD_ID && !string.IsNullOrEmpty(_puzzleToUnlock))
        {
            UnlockPuzzle(_puzzleToUnlock);
            _puzzleToUnlock = null; // Сбрасываем
        }
    }

    // Разблокировка пазла в сохранениях
    private void UnlockPuzzle(string puzzleId)
    {
        //if (!YG2.saves.unlockedPuzzles.Contains(puzzleId))
        //{
        //    YG2.saves.unlockedPuzzles.Add(puzzleId);
        //    YG2.SaveProgress();
        //    Debug.Log($"Puzzle unlocked: {puzzleId}");
        //}
    }
}