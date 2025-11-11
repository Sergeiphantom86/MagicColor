using UnityEngine;
using YG;

public class PauseMenu : MonoBehaviour
{
    private int _speedTimePassing;

    private void Awake()
    {
        _speedTimePassing = 1;
    }

    public void ResumeGame()
    {
        SwitchTime(false, _speedTimePassing);
    }

    public void PauseGame()
    {
        SwitchTime(true, _speedTimePassing - _speedTimePassing);
    }

    public void LoadMenu()
    {
        SwitchTime(false, _speedTimePassing);
        SceneLoader.Instance.ExitToMainMenu();
        YG2.SaveProgress();
    }

    private void SwitchTime(bool isOn, int speedTimePassing)
    {
        gameObject.SetActive(isOn);
        Time.timeScale = speedTimePassing;
    }
}