using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DustSizeCalculator))]
public class AnimatorPenFilling : MonoBehaviour
{
    [SerializeField] private FragmentSpawner _fragmentSpawner;

    private DustSizeCalculator _dustSizeCalculator;
    private float _duration;
    private Vector3 _initialScale;
    private float _currentOccupancy;

    public int Size { get; private set; }

    private void Awake()
    {
        _duration = 1f;
        _initialScale = new Vector3(12, 0, 12);
        _dustSizeCalculator = GetComponent<DustSizeCalculator>();
        _currentOccupancy = 0f;
    }

    public void UpdatePenSize(int quantity, Placeholder placeholder)
    {
        if (placeholder == null || quantity < 0) return;

        Size = quantity;
        _currentOccupancy = GetQuantityOccupancy(quantity);

        ChangeSize(placeholder, _currentOccupancy);
        ChangePosition(placeholder, _currentOccupancy);
    }

    public void FillPen(Color color, Placeholder placeholder)
    {
        int fragmentCount = GetFragmentCount(color);
        UpdatePenSize(fragmentCount, placeholder);
    }

    private int GetFragmentCount(Color color)
    {
        if (_fragmentSpawner == null || _fragmentSpawner.Fragments == null)
        {
            Debug.LogError("FragmentSpawner or Fragments dictionary is null!", this);
            return 0;
        }

        if (_fragmentSpawner.Fragments.TryGetValue(color, out Queue<Fragment> fragments))
        {
            return fragments?.Count ?? 0;
        }

        return 0;
    }

    public float GetDuration() => _duration;

    public void ChangeSize(Placeholder placeholder, float occupancy)
    {
        placeholder.transform.DOScale(
           GetNewScaleY(occupancy), _duration)
           .SetEase(Ease.OutQuad);
    }

    private void ChangePosition(Placeholder placeholder, float occupancy)
    {
        placeholder.transform.DOLocalMove(
           GetPosition(placeholder.transform.localPosition,
           GetHeightIncrease(occupancy)), _duration)
           .SetEase(Ease.OutQuad);
    }

    private Vector3 GetNewScaleY(float occupancy)
    {
        return new(
            _initialScale.x,
            _initialScale.y + occupancy,
            _initialScale.z
        );
    }

    private Vector3 GetPosition(Vector3 initialPosition, float heightIncrease)
    {
        return new(
            initialPosition.x,
            initialPosition.y + heightIncrease,
            initialPosition.z
        );
    }

    private float GetHeightIncrease(float occupancy)
    {
        return GetNewScaleY(occupancy).y - _initialScale.y;
    }

    private float GetQuantityOccupancy(int quantity)
    {
        if (_dustSizeCalculator == null)
        {
            Debug.LogError("DustSizeCalculator is null!");
            return 0f;
        }

        if (_fragmentSpawner == null)
        {
            Debug.LogError("FragmentSpawner is null!");
            return 0f;
        }

        if (quantity < 0)
        {
            Debug.LogError("quantity меньше 0!");
            return 0f;
        }

        return _dustSizeCalculator.CalculateSize(quantity, _fragmentSpawner.TotalCount);
    }
}