using UnityEngine;
using YG;
using static SceneLoader;

[RequireComponent(typeof(PanelFader))]
public class MenuController : MonoBehaviour
{
    private const string AfterPuzzleRewardID = "after_puzzle_reward";
    private const string Menu = nameof(Menu);

    [SerializeField] private MenuButtons _menuButtons;
    [SerializeField] private AnimatorPuzzle _animation;
    [SerializeField] private ImageAnalyzer _imageAnalyzer;
    [SerializeField] private Timer _timer;
    [SerializeField] private PanelFader _panelFader;

    private bool _adInProgress;

    private void Awake()
    {
        ValidateComponents();
        _menuButtons.Initialize(HandleStartButton, HandleResumeButton);
    }

    private void OnEnable()
    {
        if (_animation != null)
            _animation.PuzzleIsComplete += HandlePuzzleComplete;

        YG2.onCloseRewardedAdv += OnAdClosed;
        YG2.onErrorRewardedAdv += OnAdError;
    }

    private void OnDisable()
    {
        if (_animation != null)
            _animation.PuzzleIsComplete -= HandlePuzzleComplete;

        YG2.onCloseRewardedAdv -= OnAdClosed;
        YG2.onErrorRewardedAdv -= OnAdError;
    }

    private void OnDestroy()
    {
        _menuButtons.CleanUp();
    }

    private void HandleStartButton()
    {
        if (YG2.saves.Sprite == null)
        {
            Debug.LogWarning("Sprite is missing in EntryPoint");
        }

        _imageAnalyzer.AnalyzeTexture(YG2.saves.Sprite);
        _panelFader.FadeOut(() => _menuButtons.HideStartButton());
        _timer.StartTimer();
    }

    private void HandlePuzzleComplete()
    {
        _panelFader.FadeIn(() => _menuButtons.ShowResumeButton());
    }

    private void HandleResumeButton()
    {
        if (TryShowAd())
        {
            _adInProgress = true;
        }
        else
        {
            LoadMenuScene();
        }
    }

    private bool TryShowAd()
    {
        if (YG2.nowRewardAdv ==false && YG2.nowAdsShow == false)
        {
            YG2.RewardedAdvShow(AfterPuzzleRewardID, null);
            return true;
        }
        return false;
    }

    private void OnAdClosed()
    {
        if (_adInProgress)
        {
            LoadMenuScene();
        }
    }

    private void OnAdError()
    {
        if (_adInProgress)
        {
            Debug.LogError("Rewarded ad error");
            LoadMenuScene();
        }
    }

    private void LoadMenuScene()
    {
        _adInProgress = false;
        YG2.saves.ResetSprite();
        Instance.LoadSceneWithSplash(Menu);
    }

    private void ValidateComponents()
    {
        if (_imageAnalyzer == null)
            Debug.LogWarning("ImageAnalyzer не назначен", this);

        if (_animation == null)
            Debug.LogWarning("AnimatorPuzzle не назначен", this);

        if (_panelFader == null)
            Debug.LogWarning("PanelFader не назначен", this);
    }
}