using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PixelMover : MonoBehaviour, IAnimatable
{
    [Header("Settings")]
    [SerializeField] private float _moveDistance;
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _delayBetweenPixels;

    private Sorter _sorter;
    private Sequence _currentSequence;
    private List<Fragment> _sortedPixels;

    public event Action<List<Fragment>> HasDisplaced;

    private void Awake()
    {
        _sortedPixels = new List<Fragment>();
        _sorter = GetComponent<Sorter>();
    }

    private void OnEnable()
    {
        _sorter.HasSorted += Animate;
    }

    private void OnDisable()
    {
        _sorter.HasSorted -= Animate;
    }

    public void PauseAnimations()
    {
        if (_currentSequence != null && _currentSequence.IsPlaying())
        {
            _currentSequence.Pause();
        }
    }

    public void ResumeAnimations()
    {
        if (_currentSequence != null && _currentSequence.IsPlaying() == false && _currentSequence.IsActive())
        {
            _currentSequence.Play();
        }
    }

    private void Animate()
    {
        _currentSequence = DOTween.Sequence();

        _sortedPixels = _sorter.Fragments;

        for (int i = 0; i < _sortedPixels.Count; i++)
        {
            _currentSequence.Insert(GetDelayPixels(i), GetPixelTransform(_sortedPixels, i)
                .DOMove(GetTargetPosition(_sortedPixels, i), _animationDuration)
                .SetEase(Ease.InOutFlash))
                .OnComplete(() =>
                HasDisplaced?.Invoke(_sortedPixels));
        }
    }
 
    private Vector3 GetTargetPosition(List<Fragment> sortedPixels, int index)
    {
        return sortedPixels[index].transform.position + Vector3.down * _moveDistance;
    }

    private float GetDelayPixels(int index)
    {
        return index * _delayBetweenPixels;
    }

    private Transform GetPixelTransform(List<Fragment> sortedPixels, int index)
    {
        return sortedPixels[index].transform;
    }
}