using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowController : MonoBehaviour
{
    private const string Puzzle = nameof(Puzzle);
    private const string Roulette = nameof(Roulette);
    private const string Tutorial = nameof(Tutorial);

    [SerializeField] private MenuLoader _menuLoader;
    [SerializeField] private TextureInitializer _textureInitializer;
    [SerializeField] private TutorialPuzzle _tutorialPuzzle;
    [SerializeField] private Timer _timer;
    [SerializeField] private PuzzlesIdentifier _puzzlesIdentifier; 

    private Sprite _sprite;
    private string _sceneName;
    private IProgressSaver _progressSaver;
    private AdRewardController _adRewardController;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _progressSaver = new ProgressSaver();
        _adRewardController = GetComponent<AdRewardController>();
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        SetSprite();

        if (_sprite != null)
            _textureInitializer.SpawnPixelsFromTexture(_sprite.texture);
    }

    public void LoadNextScene()
    {
        if (_sceneName != Tutorial)
        {
            _adRewardController.ShowRewardAd(LoadSceneRoulette);
            _progressSaver.SaveProgress();
            return;
        }

        _menuLoader.TargetScene(Puzzle);
    }

    private void LoadSceneRoulette()
    {
        _menuLoader.TargetScene(Roulette);
    }

    private void SetSprite()
    {
        _sprite = _tutorialPuzzle != null
           ? _tutorialPuzzle.Sprite
           : _progressSaver.Saves?.CurrentSprite;
    }
}