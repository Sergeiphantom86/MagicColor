using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Menu.GameLogicQuests
{
    [RequireComponent(typeof(TransitionChooser))]

    public class QuestSystem : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private Quest _next;
        private Quest _active;
        private ZoomChanger _zoomChanger;
        private List<Quest> _subscribedQuests;
        private IReadOnlyList<Quest> _quests;
        private TransitionChooser _transitionChooser;
        private bool _isActivate;
        private bool _isOn;

        private void Awake()
        {
            _transitionChooser = GetComponent<TransitionChooser>();
            _subscribedQuests = new List<Quest>();
            _zoomChanger = new ZoomChanger();

            if (_transitionChooser == null)
            {
                Debug.LogError("TransitionChooser not found!");
                return;
            }
        }

        private void OnDestroy()
        {
            foreach (var quest in _subscribedQuests)
            {
                if (quest != null)
                {
                    quest.Selected -= OnSelect;
                }
            }

            _isActivate = false;
            _subscribedQuests.Clear();
            YG2.SaveProgress();
        }

        public void Initialize(IReadOnlyList<Quest> quests)
        {
            if (quests == null || quests.Count == 0)
                return;

            _quests = quests;

            _isOn = YG2.saves.IsAutomaticallyNewLevel;

            _transitionChooser.Initialize(_zoomChanger);

            ProcessSavedProgress();

            if (_isActivate)
            {
                _button.onClick.Invoke();
                return;
            }

            TurnOff();
        }

        private void TurnOff()
        {
            gameObject.SetActive(false);
        }

        private void ProcessSavedProgress()
        {
            if (IsQuestListValid() == false)
                return;

            UnlockQuestsUpToSavedIndex();
            SetActiveQuestIndicator();
            AutoTransition();
        }

        private bool IsQuestListValid()
        {
            if (_quests == null || _quests.Count == 0)
            {
                Debug.LogError("Quests list is null or empty");
                return false;
            }

            return true;
        }

        private void UnlockQuestsUpToSavedIndex()
        {
            int lastIndex = GetIndex();

            for (int i = 0; i <= lastIndex; i++)
            {
                SetupQuest(i);
            }

            _next = GetNextQuest(lastIndex);
        }

        private void SetupQuest(int index)
        {
            _active = _quests[index];

            if (_active.IsUnlocked)
                return;

            _active.Unlock();
            SubscribeToQuest(_active);
        }

        private Quest GetNextQuest(int index)
        {
            return (index + 1 < _quests.Count) ? _quests[index + 1] : null;
        }

        private void SubscribeToQuest(Quest quest)
        {
            quest.Selected += OnSelect;
            _subscribedQuests.Add(quest);
        }

        private void SetActiveQuestIndicator()
        {
            if (_active != null)
                _active.SetActiveIndicator(true);
        }

        public void AutoTransition()
        {
            if (_isOn == false || _active == null)
                return;

            _isActivate = true;
            _active.Click();
        }

        private int GetIndex()
        {
            return Mathf.Clamp(YG2.saves.MaxReachedQuestIndex, 0, _quests.Count - 1);
        }

        private void OnSelect(Quest quest)
        {
            if (_active == quest)
            {
                if (_next != null)
                {
                    YG2.saves.SetNew(_next.Sprite);
                }
            }

            YG2.saves.QuestIndex = quest.Index;

            if (_transitionChooser != null)
            {
                _transitionChooser.ChoosePuzzle(quest);

                YG2.saves.IsAutomaticallyNewLevel = false;
            }
        }
    }
}