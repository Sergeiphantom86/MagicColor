using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

[RequireComponent(typeof(PanelFader))]
public class MenuController : MonoBehaviour
{
    private const string AfterPuzzleRewardID = "after_puzzle_reward";
    private const string Puzzle = nameof(Puzzle);
    private const string Roulette = nameof(Roulette);
    private const string Tutorial = nameof(Tutorial);

    [SerializeField] private MenuButtons _menuButtons;
    [SerializeField] private AnimatorPuzzle _animation;
    [SerializeField] private Timer _timer;
    [SerializeField] private PanelFader _panelFader;
    [SerializeField] private TutorialPuzzle _tutorialPuzzle;
    [SerializeField] private ButtonHome _buttonHome;
    [SerializeField] private TextureInitializer _textureInitializer;

    private MenuLoader _menuLoader;

    private bool _adInProgress;

    private void Awake()
    {
        _menuLoader = GetComponent<MenuLoader>();

        ValidateComponents();

        _menuButtons.Initialize(HandleStartButton, HandleResumeButton);
    }

    private void OnEnable()
    {
        if (_animation != null)
            _animation.PuzzleIsComplete += HandlePuzzleComplete;

        YG2.onCloseRewardedAdv += OnAdClosed;
        YG2.onErrorRewardedAdv += OnAdClosed;
    }

    private void OnDisable()
    {
        if (_animation != null)
            _animation.PuzzleIsComplete -= HandlePuzzleComplete;

        YG2.onCloseRewardedAdv -= OnAdClosed;
        YG2.onErrorRewardedAdv -= OnAdClosed;
    }

    private void OnDestroy()
    {
        _menuButtons.CleanUp();
    }

    private void HandleStartButton()
    {
        if (_tutorialPuzzle == null)
        {
            if (YG2.saves == null && YG2.saves.Sprite == null)
            {
                Debug.LogError("YG2.saves is null!");
                return;
            }

            LoadScene(YG2.saves.Sprite);
            return;
        }
        
        LoadScene(_tutorialPuzzle.Sprite);
    }

    private void LoadScene(Sprite sprite)
    {
        _textureInitializer.SpawnPixelsFromTexture(sprite.texture);
        _panelFader.FadeOut(() => 
        _menuButtons.HideStartButton());

        if (_timer != null)
        {
            _timer.StartTimer();
        }
    }

    private void HandlePuzzleComplete()
    {
        _menuButtons.ShowResumeButton();
        _buttonHome.gameObject.SetActive(false);
    }

    private void HandleResumeButton()
    {
        _panelFader.FadeIn(() =>
        {
            if (TryShowAd())
            {
                _adInProgress = true;
            }
            else
            {
                OnAdClosed();
            }
        });
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

    private void LoadMenuScene()
    {
        _adInProgress = false;

        if (SceneManager.GetActiveScene().name != Tutorial)
        {
            _menuLoader.TargetScene(Roulette);
            return;
        }

        _menuLoader.TargetScene(Puzzle);
    }

    private void ValidateComponents()
    {
        if (_textureInitializer == null)
            Debug.LogWarning("TextureInitializer не назначен", this);

        if (_animation == null)
            Debug.LogWarning("AnimatorPuzzle не назначен", this);

        if (_panelFader == null)
            Debug.LogWarning("PanelFader не назначен", this); 
    }
}