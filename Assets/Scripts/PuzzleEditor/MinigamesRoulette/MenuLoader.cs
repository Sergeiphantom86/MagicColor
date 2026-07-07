using Game.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace PuzzleEditor.MinigamesRoulette
{
    public class MenuLoader : MonoBehaviour
    {
        private const string Menu = nameof(Menu);

        public void TargetScene(string scenName)
        {
            if (YG2.saves.SceneLoader == null)
            {
                Debug.LogError("SceneLoader instance not found! Using default load.");
                SceneManager.LoadScene(Menu);
                return;
            }

            YG2.saves.SceneLoader.LoadSceneAsyncWithSplash(scenName);
        }
    }
}