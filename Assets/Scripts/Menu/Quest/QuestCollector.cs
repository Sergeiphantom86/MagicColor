using System.Collections.Generic;
using UnityEngine;
using YG;

public class QuestCollector : MonoBehaviour
{
    [SerializeField] private QuestSystem _questSystem;
    [SerializeField] private Viewer _viewer;

    private List<Quest> _allQuests;

    private void Awake()
    {
        _allQuests = new List<Quest>();
    }

    private void Start()
    {
        CollectChildQuests();
    }

    private void CollectChildQuests()
    {
        _allQuests.Clear();
        
        for (int i = 0; i < _questSystem.transform.childCount; i++)
        {
            if (_questSystem.transform.GetChild(i).TryGetComponent(out Quest quest))
            {
                _allQuests.Add(GetCustomized(quest, i));
            }
        }

        _questSystem.Initialize(_allQuests);
    }

    private Quest GetCustomized(Quest quest, int index)
    {
        quest.SetIndex(index);

        SetTutorial(quest, YG2.saves.IsFirstTutorial, 0);
        SetTutorial(quest, YG2.saves.IsSecondTutorial, YG2.saves.IndexUnblocking);

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