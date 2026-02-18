using System.Collections.Generic;
using UnityEngine;

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
        _progressSaver = new ProgressSaver();
        _questCustomizer = new QuestCustomizer(_progressSaver);

        ValidateDependencies();
    }

    private void Start()
    {
        InitializeQuests();
        _questSystem.gameObject.SetActive(false);
    }

    private void InitializeQuests()
    {
        ClearCollections();
        CollectQuests();
        SaveQuestProgress();
        SetupSprites();
        _questCustomizer.Apply(_allQuests);
        InitializeQuestSystem();
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

    private void InitializeQuestSystem()
    {
        _questSystem.Initialize(_allQuests);
    }

    private void SetLatestSprite()
    {
        if (_spritesQuests == null || _spritesQuests.Count == 0)
            return;

        int index = Mathf.Clamp(_progressSaver.Saves.QuestIndex, 0, _spritesQuests.Count - 1);

        _progressSaver.SetNewSprite(_spritesQuests[index]);
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