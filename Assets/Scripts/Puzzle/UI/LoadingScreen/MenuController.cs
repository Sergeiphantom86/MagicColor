using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuButtons _menuButtons;
    [SerializeField] private PanelFader _panelFader;
    [SerializeField] private FireworksController _fireworks;

    private SceneFlowController _sceneFlow;
    private PuzzleFlowController _puzzleFlow;

    private void Awake()
    {
        _sceneFlow = GetComponent<SceneFlowController>();
        _puzzleFlow = GetComponent<PuzzleFlowController>();

        _menuButtons.Initialize(OnAnyButton, OnResumeClicked);

        if (_menuButtons == null)
        {
            Debug.LogError("MenuButtons == null");
        }

        if (_panelFader == null)
        {
            Debug.LogError("PanelFader == null");
        }

        if (_fireworks == null)
        {
            Debug.LogError("FireworksController == null");
        }

        if (_sceneFlow == null)
        {
            Debug.LogError("SceneFlowController == null");
        }

        if (_puzzleFlow == null)
        {
            Debug.LogError("PuzzleFlowController == null");
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();

        _panelFader.FadeOut();
    }

    private void OnEnable()
    {
        _puzzleFlow.OnPuzzleCompleted += OnPuzzleCompleted;
    }

    private void OnDisable()
    {
        _puzzleFlow.OnPuzzleCompleted -= OnPuzzleCompleted;
    }

    private void OnAnyButton()
    {
    }

    private void OnPuzzleCompleted()
    {
        _menuButtons.ShowResumeButton();
    }

    private void OnResumeClicked()
    {
        _fireworks.Stop();

        _panelFader.FadeIn(() =>
        {
            _sceneFlow.LoadNext();
        });
    }
}