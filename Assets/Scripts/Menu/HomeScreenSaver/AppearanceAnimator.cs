using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Sorter))]
public class AppearanceAnimator : MonoBehaviour, IAnimatable
{
    private float _animationDuration;
    private float _delayBetweenObjects;
    private Vector3 _startScale;
    private Vector3 _endScale;
    private Sorter _sorter;
    private Sequence _currentSequence;

    private List<Fragment> _fragments;

    public List<Fragment> Fragments => _fragments;

    public event Action OnAppearanceComplete;

    private void Awake()
    {
        _startScale = Vector3.one / 2;
        _endScale = new Vector3(20, 20, 1f);
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
            OnAppearanceComplete?.Invoke();
        });
    }

    private void CustomizeFragment(int index, Fragment fragment)
    {
        SetInitialSize(fragment);

        fragment.gameObject.SetActive(true);

        AddAnimation(index, fragment);
    }

    private void SetInitialSize(Fragment fragment)
    {
        fragment.transform.localScale = _startScale;
    }

    private void AddAnimation(int index, Fragment fragment)
    {
        _currentSequence.Insert(
              index * _delayBetweenObjects,
              fragment.transform.DOScale(_endScale, _animationDuration).SetEase(Ease.OutBack)
          );
    }

    private void ResetAnimation()
    {
        if (_currentSequence != null && _currentSequence.IsActive())
        {
            _currentSequence.Kill();
        }

        _currentSequence = null;
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
        if (_currentSequence != null && _currentSequence.IsPlaying() == false)
        {
            _currentSequence.Play();
        }
    }
}