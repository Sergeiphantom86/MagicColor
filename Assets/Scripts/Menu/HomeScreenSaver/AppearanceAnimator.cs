using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[RequireComponent(typeof(Sorter))]
public class AppearanceAnimator : MonoBehaviour, IAnimatable
{
    private float _animationDuration;
    private float _delayBetweenObjects;
    private float _startSizeMultiplier;
    private Vector3 _endScale;
    private Sorter _sorter;
    private Sequence _currentSequence;
    private List<Fragment> _fragments;

    public List<Fragment> Fragments => _fragments;

    public event Action AppearanceCompleted;

    private void Awake()
    {
        _startSizeMultiplier = 100;
        _endScale = new Vector3(1, 1, 1f);
        _delayBetweenObjects = 0.01f;
        _animationDuration = 0.5f;
        _sorter = GetComponent<Sorter>();
        _fragments = new List<Fragment>();
    }

    private void OnEnable()
    {
        _sorter.HasSorted += AnimateAppearance;
    }

    private void OnDisable()
    {
        _sorter.HasSorted -= AnimateAppearance;
    }

    public void PauseAnimations() => 
        DOTweenExtensions.SafePause(_currentSequence);

    public void ResumeAnimations() => 
        DOTweenExtensions.SafePlay(_currentSequence);

    private void AnimateAppearance()
    {
        ResetAnimation();

        _currentSequence = DOTween.Sequence();

        _fragments = _sorter.Fragments;

        for (int i = 0; i < _fragments.Count; i++)
        {
            CustomizeFragment(i, _fragments[i]);
        }

        _currentSequence.OnComplete(() =>
        {
            AppearanceCompleted?.Invoke();
        });
    }

    private void CustomizeFragment(int index, Fragment fragment)
    {
        SetInitialSize(fragment);

        fragment.TurnOn();

        AddAnimation(index, fragment);
    }

    private void SetInitialSize(Fragment fragment)
    {
        fragment.transform.localScale = GetStartScale();
    }

    private Vector3 GetStartScale()
    {
        return Vector3.one / _startSizeMultiplier;
    }

    private void AddAnimation(int index, Fragment fragment)
    {
        _currentSequence.Insert(
            index * _delayBetweenObjects,
            fragment.transform
                .DOScale(_endScale, _animationDuration)
                .SetEase(Ease.OutBack)
                .SetLink(fragment.gameObject)
        );
    }

    private void ResetAnimation()
    {
        DOTweenExtensions.SafeKill(_currentSequence);

        if (_fragments != null)
        {
            foreach (var fragment in _fragments)
            {
                if (fragment != null)
                    fragment.transform.DOKill();
            }
        }

        _currentSequence = null;
    }
}