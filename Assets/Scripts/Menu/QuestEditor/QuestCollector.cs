using Menu.HomeScreenSaver;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Menu.QuestEditor
{
    public class QuestCollector : MonoBehaviour
    {
        [SerializeField] private QuestSystem _questSystem;
        [SerializeField] private Viewer _viewer;
        [SerializeField] private Contender _contender;

        private List<Quest> _allQuests;
        private List<Sprite> _spritesQuests;
        private QuestCustomizer _questCustomizer;

        private void Awake()
        {
            _allQuests = new List<Quest>();
            _spritesQuests = new List<Sprite>();
            ValidateDependencies();
        }

        private void Start()
        {
            Initialize();
        }


        public void Initialize()
        {
            _questCustomizer = new QuestCustomizer();

            ClearCollections();
            CollectQuests();
            SaveQuestProgress();
            SetupSprites();

            _questCustomizer.Apply(_allQuests);
            _questSystem.Initialize(_allQuests);
        }

        private void ClearCollections()
        {
            _allQuests.Clear();
            _spritesQuests.Clear();
        }

        private void SetupSprites()
        {
            SetLatestSprite();
            _viewer.AddSprite(_spritesQuests);
        }

        private void SaveQuestProgress()
        {
            YG2.saves.CountQuest = _spritesQuests.Count;
        }

        private void CollectQuests()
        {
            Transform[] childs = _contender.AllChildren;

            if (childs.Length <= 0)
            {
                Debug.LogWarning("AllChildren.Length = 0 ");
                return;
            }

            foreach (Transform child in childs)
            {
                if (child.TryGetComponent(out Quest quest))
                {
                    _allQuests.Add(quest);
                    _spritesQuests.Add(quest.Sprite);
                }
            }
        }

        private void SetLatestSprite()
        {
            if (_spritesQuests == null || _spritesQuests.Count == 0)
                return;

            int index = Mathf.Clamp(YG2.saves.QuestIndex, 0, _spritesQuests.Count - 1);

            YG2.saves.SetNew(_spritesQuests[index]);
        }

        private void ValidateDependencies()
        {
            if (_questSystem == null)
                Debug.LogError("QuestSystem is not assigned!");

            if (_viewer == null)
                Debug.LogError("Viewer is not assigned!");

            if (_contender == null)
                Debug.LogError("Contender is not assigned!");
        }
    }
}