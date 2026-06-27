using Menu;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace PuzzleEditor.RouletteEditor
{
    public class AutomaticTransitionInstaller : MonoBehaviour
    {
        [SerializeField] private ButtonHome ButtonHome;

        private Sprite _newSprite;
        private Button _nextPuzzle;
        private PuzzleSelector _selector;
        private int _firstTutorial;
        private int _secondTutorial;
        private int _thirdTutorial;
        private int _maxReachedQuestIndex;

        private void Awake()
        {
            _nextPuzzle = GetComponent<Button>();
            _selector = GetComponentInChildren<PuzzleSelector>();

            Initialized();
        }

        private void Start()
        {
            _nextPuzzle.onClick.AddListener(OnSetValue);
        }

        private void Initialized()
        {
            _newSprite = YG2.saves.New;

            _firstTutorial = 0;
            _secondTutorial = YG2.saves.IndexSecondQuest;
            _thirdTutorial = YG2.saves.ObstacleDeactivatIndex;
            _maxReachedQuestIndex = YG2.saves.MaxReachedQuestIndex;

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

        private void OnSetValue()
        {
            YG2.saves.IsAutomaticallyNewLevel = true;
            ButtonHome.GoMenu();
        }

        private void Show()
        {
            if (TryEnableFollowingQuest(_maxReachedQuestIndex) || HasMatchingQuestIndex())
            {
                gameObject.SetActive(false);
            }

            _selector.SetSprite(_newSprite);

            if (YG2.saves.QuestIndex >= YG2.saves.MaxReachedQuestIndex)
            {
                YG2.saves.MaxReachedQuestIndex++;
            }
        }

        private bool HasMatchingQuestIndex()
        {
            return _firstTutorial == _maxReachedQuestIndex
            || _secondTutorial == _maxReachedQuestIndex
            || _thirdTutorial == _maxReachedQuestIndex;
        }

        private bool TryEnableFollowingQuest(int indexCurrentQuest)
        {
            return indexCurrentQuest >= YG2.saves.CountQuest;
        }
    }
}