using UnityEngine;
using YG;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenuUI;

    public static bool IsPaused { get; private set; }

    public void ResumeGame()
    {
        _pauseMenuUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void PauseGame()
    {
        _pauseMenuUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneLoader.Instance.ExitToMainMenu();
        YG2.SaveProgress();
    }
}