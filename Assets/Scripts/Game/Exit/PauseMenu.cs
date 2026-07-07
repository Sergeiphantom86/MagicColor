using UnityEngine;
using YG;

namespace Game.Exit
{
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

            if (YG2.saves.SceneLoader == null)
            {
                Debug.LogError("SceneLoader instance not found! Using default load.");
                return;
            }

            YG2.saves.SceneLoader.LoadSceneAsyncWithSplash(sceneName);
        }

        private void SwitchTime(bool isOn, int speedTimePassing)
        {
            gameObject.SetActive(isOn);

            Time.timeScale = speedTimePassing;
        }
    }
}