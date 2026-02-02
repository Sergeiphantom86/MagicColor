using System.Collections.Generic;
using UnityEngine;
using YG;

public class QuestCollector : MonoBehaviour
{
    [SerializeField] private QuestSystem _questSystem;
    [SerializeField] private Viewer _viewer;
    [SerializeField] private Contender _contender;

    private List<Quest> _allQuests;
    private List<Sprite> _sprits;
    private int _indexFirstQuest;
    private int _indexSecondQuest;
    private bool _isFirstTutorial;
    private bool _isSecondTutorial;

    private void Awake()
    {
        _allQuests = new List<Quest>();
        _sprits = new List<Sprite>();
        _indexFirstQuest = 1;
        _indexSecondQuest = YG2.saves.IndexSecondQuest;
        _isFirstTutorial = YG2.saves.IsFirstTutorial;
        _isSecondTutorial = YG2.saves.IsSecondTutorial;

        if (_questSystem == null)
        {
            Debug.LogError("QuestSystem is not assigned in QuestCollector!");
            return;
        }

        if (_viewer == null)
        {
            Debug.LogError("Viewer is not assigned in QuestCollector!");
            return;
        }

        if (_questSystem.transform == null)
        {
            Debug.LogError("QuestSystem transform is null!");
            return;
        }
    }

    private void Start()
    {
        CollectChildQuests();
        _questSystem.gameObject.SetActive(false);    
    }

    private void CollectChildQuests()
    {
        _allQuests.Clear();
        _sprits.Clear();

        for (int i = 0; i < _contender.transform.childCount; i++)
        {
            if (GetTransformChild(i).TryGetComponent(out Quest quest))
            {
                Quest customizedQuest = GetCustomized(quest, i);

                if (customizedQuest != null)
                {
                    _allQuests.Add(customizedQuest);
                    _sprits.Add(customizedQuest.Sprite);
                }
            }
        }

        SetLatestSprite();

        YG2.saves.SetCountQuest(_sprits.Count);

        _questSystem.Initialize(_allQuests);
        _viewer.AddSprite(_sprits);
    }

    private void SetLatestSprite()
    {
        if (_sprits == null || _sprits.Count == 0)
            return;

        int index = Mathf.Clamp(YG2.saves.QuestIndex, 0, _sprits.Count - 1);

        YG2.saves.SetNewSprite(_sprits[index]);
    }

    private Transform GetTransformChild(int index)
    {
        return _contender.transform.GetChild(index);
    }

    private Quest GetCustomized(Quest quest, int index)
    {
        quest.SetIndex(index);

        SetTutorial(quest, _isFirstTutorial, _indexFirstQuest);
        SetTutorial(quest, _isSecondTutorial, _indexSecondQuest);

        return quest;
    }

    private void SetTutorial(Quest quest, bool isTutorial, int index)
    {
        if (quest.Index == index)
        {
            quest.SetTutorial(isTutorial);
        }
    }
}