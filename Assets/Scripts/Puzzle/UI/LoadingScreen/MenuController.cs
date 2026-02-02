using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MenuButtons _menuButtons;
    [SerializeField] private PanelFader _panelFader;
    [SerializeField] private ButtonHome _buttonHome;
    [SerializeField] private FireworksController _fireworks;

    private SceneFlowController _sceneFlow;
    private AdRewardController _adRewardController;
    private PuzzleFlowController _puzzleFlow;

    private void Awake()
    {
        _sceneFlow = GetComponent<SceneFlowController>();
        _adRewardController = GetComponent<AdRewardController>();
        _puzzleFlow = GetComponent<PuzzleFlowController>();

        _menuButtons.Initialize(OnAnyButton, OnResumeClicked);
    }

    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();

        _panelFader.FadeOut();
        _sceneFlow.StartGame();
    }

    private void OnEnable()
    {
        _puzzleFlow.OnPuzzleCompleted += OnPuzzleCompleted;
    }

    private void OnDisable()
    {
        _puzzleFlow.OnPuzzleCompleted -= OnPuzzleCompleted;
    }

    private void OnAnyButton() { }

    private void OnPuzzleCompleted()
    {
        _menuButtons.ShowResumeButton();
        _buttonHome.gameObject.SetActive(false);
    }

    private void OnResumeClicked()
    {
        _fireworks.Stop();

        _panelFader.FadeIn(() =>
        {
            _adRewardController.ShowRewardAd(_sceneFlow.LoadNextScene);
        });
    }
}