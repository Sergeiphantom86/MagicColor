using Game.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleEditor.RouletteEditor
{
    public class MenuLoader : MonoBehaviour
    {
        private const string Menu = nameof(Menu);

        public void TargetScene(string scenName)
        {
            if (SceneLoader.Instance == null)
            {
                Debug.LogError("SceneLoader instance not found! Using default load.");
                SceneManager.LoadScene(Menu);
                return;
            }

            SceneLoader.Instance.LoadSceneAsyncWithSplash(scenName);
        }
    }
}