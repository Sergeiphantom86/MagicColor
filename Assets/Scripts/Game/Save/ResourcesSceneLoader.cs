using UnityEngine;

public class ResourcesSceneLoader : MonoBehaviour
{
    private const string Roulette = nameof(Roulette);
    private const string Puzzle = nameof(Puzzle);
    private const string Menu = nameof(Menu);

    private SpriteTransmitter _spriteTransmitter;
    private IProgressSaver _progressSaver;
    private RouletteGameStarter _rouletteGameStarter;
    private MenuStarter _menuStarter;
    private SceneFlowController _sceneFlowController;

    private void Awake()
    {
        _spriteTransmitter = GetComponent<SpriteTransmitter>();
        _progressSaver = new ProgressSaver();
    }

    public void DownloadNecessaryResources(string nameScene)
    {
        if (Roulette == nameScene)
        {
            _rouletteGameStarter = FindObjectOfType<RouletteGameStarter>();

            _rouletteGameStarter.SetProgressSaver(_progressSaver, _spriteTransmitter.New);
        }

        if (Menu == nameScene)
        {
            _menuStarter = FindObjectOfType<MenuStarter>();

            _menuStarter.Initialize(_progressSaver, _spriteTransmitter);
        }

        if (Puzzle == nameScene)
        {
            _sceneFlowController = FindObjectOfType<SceneFlowController>();

            _sceneFlowController.Initialize(_spriteTransmitter.Current, _progressSaver);
        }
    }
}