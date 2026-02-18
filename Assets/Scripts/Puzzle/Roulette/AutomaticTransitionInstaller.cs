using UnityEngine;
using UnityEngine.UI;

public class AutomaticTransitionInstaller : MonoBehaviour
{
    [SerializeField] protected ButtonHome _buttonHome;
    
    private Button _nextPuzzle;
    private PuzzleSelector _selector;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _nextPuzzle = GetComponent<Button>();
        _selector = GetComponentInChildren<PuzzleSelector>();
        _progressSaver = new ProgressSaver();

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

    private void Start()
    {
        _nextPuzzle.onClick.AddListener(SetValue);
    }

    private void SetValue()
    {
        _progressSaver.SetAutomaticTransition(true);

        _buttonHome.GoMenu();
    }

    private void Show()
    {
        if (_progressSaver.TryEnableFollowingQuest(_progressSaver.Saves.QuestIndex + 1))
        {
            gameObject.SetActive(false);
        }
        
        if (_progressSaver.Saves.NewSprite != null && _selector != null)
        {   
            _selector.SetSprite(_progressSaver.Saves.NewSprite);
        }
    }
}