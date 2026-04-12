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

    private string _sceneName;
    private Sprite _sprite;
    private IProgressSaver _progressSaver;
    private AdRewardController _adRewardController;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _adRewardController = GetComponent<AdRewardController>();

        if (_textureInitializer == null)
            Debug.LogError($"[SceneFlowController] TextureInitializer не назначен в инспекторе на объекте {gameObject.name}");
        if (_menuLoader == null)
            Debug.LogError($"[SceneFlowController] MenuLoader не назначен на объекте {gameObject.name}");
        if (_adRewardController == null)
            Debug.LogWarning($"[SceneFlowController] AdRewardController отсутствует на объекте {gameObject.name}");
    }

    public void Initialize(Sprite sprite, IProgressSaver progressSaver)
    {
        if (sprite == null)
        {
            Debug.LogError($"Sprite == null на объекте {gameObject.name}");
            return;
        }

        if (progressSaver == null)
        {
            Debug.LogError($"IProgressSaver == null на объекте {gameObject.name}");
            return;
        }

        _progressSaver = progressSaver;
        _sprite = sprite;

        _textureInitializer.SpawnPixelsFromTexture(TryGetSprite(_sprite).texture);
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

    private Sprite TryGetSprite(Sprite sprite)
    {
        return _tutorialPuzzle != null
           ? _tutorialPuzzle.Sprite
           : sprite;
    }
}