using UnityEngine;
using UnityEngine.UI;

public class AutomaticTransitionInstaller : MonoBehaviour
{
    [SerializeField] protected ButtonHome ButtonHome;

    private Sprite _newSprite;
    private Button _nextPuzzle;
    private PuzzleSelector _selector;
    private IProgressSaver _progressSaver;
    private SpriteTransmitter _spriteTransmitter;
    private int _firstTutorial;
    private int _secondTutorial;
    private int _thirdTutorial;
    private int _maxReachedQuestIndex;

    private void Awake()
    {
        _nextPuzzle = GetComponent<Button>();
        _selector = GetComponentInChildren<PuzzleSelector>();
    }

    private void Start()
    {
        _nextPuzzle.onClick.AddListener(SetValue);
    }

    public void SetProgressSaver(IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
    {
        Initialized(progressSaver, spriteTransmitter);
    }

    private void Initialized(IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
    {
        if (progressSaver == null)
        {
            Debug.LogError("IProgressSaver == null");
            return;
        }

        _spriteTransmitter = spriteTransmitter;
        _newSprite = spriteTransmitter.New;
        _progressSaver = progressSaver;

        _firstTutorial = 0;
        _secondTutorial = _progressSaver.Saves.IndexSecondQuest;
        _thirdTutorial = _progressSaver.Saves.ObstacleDeactivatIndex;
        _maxReachedQuestIndex = _progressSaver.Saves.MaxReachedQuestIndex;

        if (_nextPuzzle == null)
        {
            Debug.LogError("Button == null");
            return;
        }

        if (_selector == null)
        {
            Debug.LogError("PuzzleSelector == null");
            return;
        }

        if (_newSprite == null)
        {
            Debug.LogError("NewSprite == null");
        }

        Show();
    }

    private void SetValue()
    {
        _spriteTransmitter.SetAutomaticTransition(true);
        ButtonHome.GoMenu();
    }

    private void Show()
    {
        if (_progressSaver.TryEnableFollowingQuest(_maxReachedQuestIndex) || HasMatchingQuestIndex())
        {
            gameObject.SetActive(false);
        }

        _selector.SetSprite(_newSprite);

        _progressSaver.SetMaxReachedQuestIndex();
    }

    private bool HasMatchingQuestIndex()
    {
        return _firstTutorial == _maxReachedQuestIndex ||
               _secondTutorial == _maxReachedQuestIndex ||
               _thirdTutorial == _maxReachedQuestIndex;
    }
}