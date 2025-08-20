using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestCollector : MonoBehaviour
{
    private List<Quest> _allQuests;
    public List<Quest> AllQuests => _allQuests;

    public event Action<List<Quest>> HasListCreated;

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

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Quest quest))
            {
                quest.SetIndex(i);
                _allQuests.Add(quest);
            }
        }

        HasListCreated?.Invoke(_allQuests);
    }
}