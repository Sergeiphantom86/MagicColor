using System.Collections;
using System.IO;
using DG.Tweening;
using PuzzleEditor.UI.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace Game.LoadingScreen
{
    [RequireComponent(typeof(CanvasGroup), typeof(PanelFader))]

    public class SceneLoader : MonoBehaviour
    {
        private const string Menu = nameof(Menu);

        [SerializeField] private float _fadeDuration;
        [SerializeField] private float _minLoadTime;

        private float _maxLoad;
        private bool _isFirstLoad;
        private bool _isInitialize;
        private PanelFader _panelFader;
        private CanvasGroup _canvasGroup;
        private Coroutine _loadingCoroutine;

        private void Awake()
        {
            YG2.saves.SceneLoader = this;

            DontDestroyOnLoad(gameObject);

            _maxLoad = 0.9f;
            _isFirstLoad = true;
            _canvasGroup = GetComponent<CanvasGroup>();
            _panelFader = GetComponent<PanelFader>();

            _canvasGroup.alpha = _isFirstLoad ? 1f : 0f;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            LoadSceneAsyncWithSplash(Menu);
        }

        public void LoadSceneAsyncWithSplash(string sceneName)
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
            }

            _loadingCoroutine = StartCoroutine(LoadAsyncSceneProcess(sceneName));
        }

        private IEnumerator LoadAsyncSceneProcess(string sceneName)
        {
            if (_panelFader != null)
                yield return _panelFader.Fade(1f, true).WaitForCompletion();

            float loadStartTime = Time.realtimeSinceStartup;

            if (ValidateSceneExists(sceneName) == false)
            {
                if (_panelFader != null)
                    yield return _panelFader.Fade(0, false).WaitForCompletion();
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < _maxLoad || (Time.realtimeSinceStartup - loadStartTime) < _minLoadTime)
            {
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;

            yield return null;

            if (_isInitialize == false)
            {
                YG2.GameReadyAPI();

                _isInitialize = true;
            }

            if (_panelFader != null)
                yield return _panelFader.Fade(0, false).WaitForCompletion();

            _isFirstLoad = false;
        }

        private bool ValidateSceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (Path.GetFileNameWithoutExtension(scenePath) == sceneName)
                    return true;
            }

            SceneManager.LoadSceneAsync(Menu);
            Debug.LogError($"Scene '{sceneName}' not found in build settings!");
            return false;
        }

        private void OnDestroy()
        {
            _canvasGroup.DOKill();
        }
    }
}