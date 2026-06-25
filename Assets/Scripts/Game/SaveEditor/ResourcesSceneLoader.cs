using PuzzleEditor.RouletteEditor;
using PuzzleEditor.UI.LoadingScreen;
using UnityEngine;
namespace Game.SaveEditor
{

public class ResourcesSceneLoader : MonoBehaviour
{
    private const string Roulette = nameof( Roulette);
    private const string Tutorial = nameof(Tutorial);
    private const string Puzzle = nameof(Puzzle);
    private const string Menu = nameof(Menu);

    private SpriteTransmitter _spriteTransmitter;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _spriteTransmitter = GetComponent<SpriteTransmitter>();

        if (_spriteTransmitter == null)
        {
            Debug.LogError($"{nameof(SpriteTransmitter)} component missing on {gameObject.name}");
        }

        _progressSaver = new ProgressSaver();
    }

    public void GoOver(string sceneName)
    {
        switch (sceneName)
        {
            case Roulette:
                InitializeRouletteScene();
                break;

            case Menu:
                InitializeMenuScene();
                break;

            case Puzzle:
            case Tutorial:
                InitializePuzzleOrTutorialScene();
                break;

            default:
                Debug.LogWarning($"No specific initialization defined for scene: {sceneName}");
                break;
        }
    }

    private void InitializeRouletteScene()
    {
        var rouletteStarter = FindObjectOfType<RouletteGameStarter>();

        if (rouletteStarter == null)
        {
            Debug.LogError($"{nameof(RouletteGameStarter)} not found in Roulette scene.");
            return;
        }

        rouletteStarter.SetProgressSaver(_progressSaver, _spriteTransmitter);
    }

    private void InitializeMenuScene()
    {
        var menuStarter = FindObjectOfType<MenuStarter>();

        if (menuStarter == null)
        {
            Debug.LogError($"{nameof(MenuStarter)} not found in Menu scene.");
            return;
        }

        menuStarter.Initialize(_progressSaver, _spriteTransmitter);
    }

    private void InitializePuzzleOrTutorialScene()
    {
        var sceneFlow = FindObjectOfType<SceneFlowController>();

        if (sceneFlow == null)
        {
            Debug.LogError($"{nameof(SceneFlowController)} not found in {Puzzle} or {Tutorial} scene.");
            return;
        }

        sceneFlow.Initialize(_spriteTransmitter.Current, _progressSaver);
    }
}
}