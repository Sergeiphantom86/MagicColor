using UnityEngine;
using UnityEngine.UI;
using YG;

public class AutomaticTransitionInstaller : MonoBehaviour
{
    [SerializeField] protected ButtonHome _buttonHome;
    
    private Button _nextPuzzle;
    private PuzzleSelector _selector;

    private void Awake()
    {
        _nextPuzzle = GetComponent<Button>();
        _selector = GetComponentInChildren<PuzzleSelector>();

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
        YG2.saves.SetAutomaticTransition(true);

        _buttonHome.GoMenu();
    }

    private void Show()
    {
        if (YG2.saves.TryEnableFollowingQuest(YG2.saves.QuestIndex + 1))
        {
            gameObject.SetActive(false);
        }
        
        if (YG2.saves.NewSprite != null && _selector != null)
        {   
            _selector.SetSprite(YG2.saves.NewSprite);
        }
    }
}