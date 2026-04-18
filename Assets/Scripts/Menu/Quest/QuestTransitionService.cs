using UnityEngine;

public class QuestTransitionService : IQuestTransitionService
{
    private const string Puzzle = nameof(Puzzle);

    private readonly SpriteTransmitter _spriteTransmitter;
    private readonly IProgressSaver _progressSaver;
    private readonly ZoomChanger _zoomChanger;
    private readonly int _transparentIndex = 2;

    private TransitionResult _result;

    public QuestTransitionService(IProgressSaver progressSaver, ZoomChanger zoomChanger, SpriteTransmitter spriteTransmitter)
    {
        _spriteTransmitter = spriteTransmitter;
        _progressSaver = progressSaver;
        _zoomChanger = zoomChanger;
        _result = new();
    }

    public TransitionResult ProcessQuest(Quest quest)
    {
        if (quest.Index == _transparentIndex)
        {
            _progressSaver.MakeTransparent(true);
        }
        
        if (quest.IsTutorial == false)
        {
            quest.SetTutorial(true);
            _progressSaver.SetTutorial(quest.Index);

            _result.ShowOffer = true;
            _result.UseMobilePanel = _zoomChanger.IsMobileWithTallScreen();

            return _result;
        }

        _progressSaver.ObstacleSwitch(true);

        if (quest.Index < _progressSaver.Saves.ObstacleDeactivatIndex)
            _progressSaver.ObstacleSwitch(false);
        else
            _progressSaver.ObstacleSwitch(true);

        _result.SceneName = Puzzle;

        return _result;
    }

    public void SaveSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            _spriteTransmitter.SetCurrent(sprite);
        }
        else
        {
            Debug.LogWarning("Cached sprite is null during transition.");
        }
    }
}