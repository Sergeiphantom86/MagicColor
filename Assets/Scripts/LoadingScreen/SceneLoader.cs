using DG.Tweening;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup), typeof(PanelFader))]
public class SceneLoader : MonoBehaviour
{
    private const string Loading = nameof(Loading);
    private const string Menu = nameof(Menu);
    private const string PuzzleScene = nameof(PuzzleScene);

    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _minLoadTime;

    private float _maxLoad;
    private bool _isFirstLoad;
    private PanelFader _panelFader;
    private CanvasGroup _canvasGroup;
    private Coroutine _loadingCoroutine;

    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _maxLoad = 0.9f;
        _isFirstLoad = true;
        _canvasGroup = GetComponent<CanvasGroup>();
        _panelFader = GetComponent<PanelFader>();

        _canvasGroup.alpha = _isFirstLoad ? 1f : 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        LoadSceneWithSplash(Menu);
    }

    public void LoadSceneWithSplash(string sceneName)
    {
        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
        }

        _loadingCoroutine = StartCoroutine(LoadSceneProcess(sceneName));
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(Menu);
    }

    private IEnumerator LoadSceneProcess(string sceneName)
    {
        yield return _panelFader.Fade(1f, true).WaitForCompletion();

        float loadStartTime = Time.realtimeSinceStartup;

        if (ValidateSceneExists(sceneName) == false)
        {
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

        yield return _panelFader.Fade(0, false).WaitForCompletion();

        _isFirstLoad = false;
    }

    private bool ValidateSceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (Path.GetFileNameWithoutExtension(scenePath) == sceneName) return true;
        }

        Debug.LogError($"—цена '{sceneName}' не найдена в настройках сборки!");
        return false;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        _canvasGroup.DOKill();
    }
}