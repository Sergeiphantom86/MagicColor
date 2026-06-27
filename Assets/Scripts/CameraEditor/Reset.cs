using Game.LoadingScreen;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CameraEditor
{
    public class Reset : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnTurnOn);
        }

        private void OnTurnOn()
        {
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            SceneLoader.Instance.LoadSceneAsyncWithSplash("Menu");
        }
    }
}