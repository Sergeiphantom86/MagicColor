using UnityEngine;
using UnityEngine.UI;
using YG;

namespace RenderingCamera
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

            if (YG2.saves.SceneLoader == null)
            {
                Debug.LogError("SceneLoader instance not found! Using default load.");
                return;
            }

            YG2.saves.SceneLoader.LoadSceneAsyncWithSplash("Menu");
        }
    }
}