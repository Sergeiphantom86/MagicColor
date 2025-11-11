using UnityEngine;
using System.Collections.Generic;

public class ItemCollector : MonoBehaviour
{
    private List<Currency> _items;

    public List<Currency> Items => _items;

    private void Awake()
    {
        _items = new List<Currency>();
        CollectChildItems();
    }

    private void CollectChildItems()
    {
        _items.Clear();
        _items.AddRange(GetComponentsInChildren<Currency>(true));
    }
}