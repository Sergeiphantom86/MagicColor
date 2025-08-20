using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouletteSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WheelAnimator _wheelAnimator;
    [SerializeField] private ButtonController _spinButtonController;
    [SerializeField] private ItemCollector _itemCollector;
    [SerializeField] private Counter _counter;
    [SerializeField] private Arrow _arrow;
    [SerializeField] private ParticleSystemController _explosionImplosion;

    private List<Award> _items;
    private bool _isSpinning;

    private void Awake()
    {
        if (_wheelAnimator == null)
            Debug.LogError($"{nameof(WheelAnimator)} ссылка не установлена!", this);

        if (_spinButtonController == null)
            Debug.LogError($"{nameof(ButtonController)} ссылка не установлена!", this);

        if (_itemCollector == null)
            Debug.LogError($"{nameof(ItemCollector)} ссылка не установлена!", this);

        if (_counter == null)
            Debug.LogError($"{nameof(Counter)} ссылка не установлена!", this);
    }

    private void Start()
    {
        _spinButtonController.Initialize(
           globalInteractableCondition: () => _counter.HasAttempts,
           onClickAction: Spin
       );

        if (_itemCollector == null) return;

        _items = _itemCollector.Items;

        for (var i = 0; i < _items.Count; i++)
        {
            _items[i].Initialize(i, _items.Count);
        }

        UpdateButtonState();
    }

    private bool HasValidItems() => 
        _items?.Count > 0;

    private void Spin()
    {
        if (_isSpinning || HasValidItems() == false || _counter.HasAttempts == false) return;
        
        _counter.DecreaseCount();

        _isSpinning = true;
        _spinButtonController.SetLocalBlock(true);

        Award result = GetPrize();

        _wheelAnimator.SpinToTarget(result.GetAngle(), () =>
        {
            _isSpinning = false;
            _spinButtonController.SetLocalBlock(false);
            _explosionImplosion.ActivateAtPosition(result);
        });
    }

    private Award GetPrize()
    {
        int cumulative = 0;

        foreach (Award item in _items)
        {
            cumulative += item.Weight;

            if (GetRandomValue(GetTotalWeight()) <= cumulative) return item;
        }

        Debug.LogWarning("Prize selection failed, returning first item");
        return _items.First();
    }

    private void UpdateButtonState()
    {
        _spinButtonController.UpdateState();
    }

    private int GetRandomValue(int totalWeight)
    {
        return Random.Range(1, totalWeight + 1);
    }

    private int GetTotalWeight()
    {
        return _items.Sum(item => item.Weight);
    }
}