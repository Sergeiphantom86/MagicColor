using UnityEngine;
using YG;

public class Altar : MonoBehaviour
{
    private const string PuzzleScene = nameof(PuzzleScene);

    [SerializeField] private GameSaveSystem _gameSaveSystem;
    private string _currentPuzzleId;

    public void GoToAnotherScene(Player player)
    {
        Sprite puzzleSprite = player.GetSprite();
        _currentPuzzleId = $"puzzle_{puzzleSprite.name}";

        YG2.saves.SetSprite(puzzleSprite);
        player.ResetSprite();

        // Сохраняем ID пазла для последующего сохранения результата
        PuzzleDataTransfer.CurrentPuzzleId = _currentPuzzleId;

        SceneLoader.Instance.LoadSceneWithSplash(PuzzleScene);
    }

    // Вызывается при возврате из сцены пазла
    public void HandlePuzzleResult(float time)
    {
        if (string.IsNullOrEmpty(_currentPuzzleId)) return;

        // Обновляем лучший результат, если нужно
        if (YG2.saves.PuzzleBestTimes.ContainsKey(_currentPuzzleId))
        {
            if (time < YG2.saves.PuzzleBestTimes[_currentPuzzleId])
            {
                YG2.saves.PuzzleBestTimes[_currentPuzzleId] = time;
            }
        }
        else
        {
            YG2.saves.PuzzleBestTimes.Add(_currentPuzzleId, time);
        }

        // Сбрасываем ID
        _currentPuzzleId = null;
    }
}