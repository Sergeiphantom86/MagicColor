using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class SceneFlowController : MonoBehaviour
{
    private const string Puzzle = nameof(Puzzle);
    private const string Roulette = nameof(Roulette);
    private const string Tutorial = nameof(Tutorial);

    [SerializeField] private MenuLoader _menuLoader;
    [SerializeField] private TextureInitializer _textureInitializer;
    [SerializeField] private TutorialPuzzle _tutorialPuzzle;
    [SerializeField] private Timer _timer;

    private Sprite _sprite;
    private string _sceneName;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
    }

    public void StartGame()
    {
        SetSprite();

        if (_sprite != null)
            _textureInitializer.SpawnPixelsFromTexture(_sprite.texture);
    }

    public void LoadNextScene()
    {
        if (_sceneName != Tutorial)
        {
            _menuLoader.TargetScene(Roulette);
            YG2.SaveProgress();
            return;
        }

        _menuLoader.TargetScene(Puzzle);
    }

    private void SetSprite()
    {
        _sprite = _tutorialPuzzle != null
           ? _tutorialPuzzle.Sprite
           : YG2.saves?.CurrentSprite;
    }
}