using UnityEngine;
using UnityEngine.UI;

public class AutomaticTransitionInstaller : MonoBehaviour
{
    [SerializeField] protected ButtonHome _buttonHome;

    private Button _nextPuzzle;
    private PuzzleSelector _selector;
    private IProgressSaver _progressSaver;
    private Sprite _newSprite;

    private void Awake()
    {
        _nextPuzzle = GetComponent<Button>();
        _selector = GetComponentInChildren<PuzzleSelector>();
    }

    private void Start()
    {
        _nextPuzzle.onClick.AddListener(SetValue);
    }

    public void SetProgressSaver(IProgressSaver progressSaver, Sprite newSprite)
    {
        Initialized(progressSaver, newSprite);
    }

    private void Initialized(IProgressSaver progressSaver, Sprite newSprite)
    {
        if (progressSaver == null)
        {
            Debug.LogError("IProgressSaver == null");
            return;
        }

        _newSprite = newSprite;
        _progressSaver = progressSaver;

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

        Show();
    }

    private void SetValue()
    {
        _progressSaver.SetAutomaticTransition(true);

        _buttonHome.GoMenu();
    }

    private void Show()
    {
        if (_progressSaver.TryEnableFollowingQuest(_progressSaver.Saves.QuestIndex))
        {
            gameObject.SetActive(false);
        }

        if (_newSprite == null)
        {
            Debug.LogError("NewSprite == null");
        }

        if (_newSprite != null && _selector != null)
        {
            _selector.SetSprite(_newSprite);
        }
    }
}