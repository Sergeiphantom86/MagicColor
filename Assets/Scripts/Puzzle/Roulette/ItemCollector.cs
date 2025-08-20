using UnityEngine;
using System.Collections.Generic;

public class ItemCollector : MonoBehaviour
{
    private List<Award> _items;

    public List<Award> Items => _items;

    private void Awake()
    {
        _items = new List<Award>();
        CollectChildItems();
    }

    private void CollectChildItems()
    {
        _items.Clear();
        _items.AddRange(GetComponentsInChildren<Award>(true));
    }
}