using System.Collections.Generic;
using Game.SaveEditor;
using Menu.HomeScreenSaver;
using UnityEngine;

namespace Menu.QuestEditor
{
    public class QuestCollector : MonoBehaviour
    {
        [SerializeField] private QuestSystem _questSystem;
        [SerializeField] private Viewer _viewer;
        [SerializeField] private Contender _contender;

        private List<Quest> _allQuests;
        private List<Sprite> _spritesQuests;
        private IProgressSaver _progressSaver;
        private QuestCustomizer _questCustomizer;

        private void Awake()
        {
            _allQuests = new List<Quest>();
            _spritesQuests = new List<Sprite>();

            ValidateDependencies();
        }

        public void Initialize(IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
        {
            _progressSaver = progressSaver;
            _questCustomizer = new QuestCustomizer(progressSaver);

            ClearCollections();
            CollectQuests();
            SaveQuestProgress();
            SetupSprites(spriteTransmitter);

            _questCustomizer.Apply(_allQuests);
            _questSystem.Initialize(_allQuests, progressSaver, spriteTransmitter);
        }

        private void ClearCollections()
        {
            _allQuests.Clear();
            _spritesQuests.Clear();
        }

        private void SetupSprites(SpriteTransmitter spriteTransmitter)
        {
            SetLatestSprite(spriteTransmitter);
            _viewer.AddSprite(_spritesQuests);
        }

        private void SaveQuestProgress()
        {
            _progressSaver.SetCountQuest(_spritesQuests.Count);
        }

        private void CollectQuests()
        {
            foreach (Transform child in _contender.transform)
            {
                if (child.TryGetComponent(out Quest quest))
                {
                    _allQuests.Add(quest);
                    _spritesQuests.Add(quest.Sprite);
                }
            }
        }

        private void SetLatestSprite(SpriteTransmitter spriteTransmitter)
        {
            if (_spritesQuests == null || _spritesQuests.Count == 0)
            return;

            int index = Mathf.Clamp(_progressSaver.Saves.QuestIndex, 0, _spritesQuests.Count - 1);

            spriteTransmitter.SetNew(_spritesQuests[index]);
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