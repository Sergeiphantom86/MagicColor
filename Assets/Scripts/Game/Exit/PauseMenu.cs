using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private int _speedTimePassing;
    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        _speedTimePassing = 1;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Resume()
    {
        _isPaused = false; 

        SwitchTime(false, _speedTimePassing);
    }

    public void Stop()
    {
        _isPaused = true;

        SwitchTime(true, _speedTimePassing - _speedTimePassing);
    }

    public void Load(string sceneName)
    {
        SwitchTime(false, _speedTimePassing);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneAsyncWithSplash(sceneName);
        }
    }

    private void SwitchTime(bool isOn, int speedTimePassing)
    {
        gameObject.SetActive(isOn);

        Time.timeScale = speedTimePassing;
    }
}