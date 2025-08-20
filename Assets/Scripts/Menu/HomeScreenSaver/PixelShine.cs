using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Agitator), typeof(AppearanceAnimator))]
public class PixelShine : MonoBehaviour, IAnimatable
{
    private float _shineDuration;
    private float _pauseBetweenPasses;
    private float _delayBetweenPixels;
    private Sequence _shineSequence;
    private Agitator _pixelExplosion;
    private AppearanceAnimator _appearanceAnimator;
    private List<Fragment> _validFragments;
    private Dictionary<Fragment, Color> _originalColors;

    private void Awake()
    {
        _shineDuration = 0.02f;
        _pauseBetweenPasses = 0.5f;
        _delayBetweenPixels = 0.001f;
        _pixelExplosion = GetComponent<Agitator>();
        _appearanceAnimator = GetComponent<AppearanceAnimator>();

        _validFragments = new List<Fragment>();
        _originalColors = new Dictionary<Fragment, Color>();
    }

    private void OnEnable() =>
        _appearanceAnimator.OnAppearanceComplete += StartShineAnimation;

    private void OnDisable()
    {
        _appearanceAnimator.OnAppearanceComplete -= StartShineAnimation;
        RestoreOriginalColors();
        _shineSequence?.Kill();
    }

    public void PauseAnimations() =>
        _shineSequence?.Pause();

    public void ResumeAnimations() =>
        _shineSequence?.Play();

    private void StartShineAnimation()
    {
        if (_appearanceAnimator.Fragments.Count == 0) return;

        _validFragments = _appearanceAnimator.Fragments
            .Where(f => f != null && f.Renderer != null)
            .ToList();

        if (_validFragments.Count == 0) return;

        StoreOriginalColors();
        CreateShineSequence();

        _shineSequence.OnComplete(() =>
        {
            RestoreOriginalColors();
            _pixelExplosion.TriggerExplosion(_validFragments);
        });

        _shineSequence.Play();
    }

    private void StoreOriginalColors()
    {
        _originalColors.Clear();

        _originalColors = _validFragments.ToDictionary(
            fragment => fragment,
            fragment => fragment.Renderer.color
        );
    }

    private void RestoreOriginalColors()
    {
        foreach (var pair in _originalColors)
        {
            if (pair.Key?.Renderer != null)
                pair.Key.Renderer.color = pair.Value;
        }

        _originalColors.Clear();
    }

    private void CreateShineSequence()
    {
        _shineSequence?.Kill();
        _shineSequence = DOTween.Sequence();

        for (int i = 0; i < _validFragments.Count; i++)
        {
            _shineSequence.Insert(i * _delayBetweenPixels, CreateShineTween(_validFragments[i]));
        }

        _shineSequence.AppendInterval(_pauseBetweenPasses);
    }

    private Sequence CreateShineTween(Fragment fragment)
    {
        SpriteRenderer renderer = fragment.Renderer;

        return DOTween.Sequence()
            .Append(renderer.DOColor(Color.white + Color.yellow, _shineDuration).SetEase(Ease.OutQuad))
            .Append(renderer.DOColor(renderer.color, _shineDuration).SetEase(Ease.InQuad));
    }

    private void OnDestroy()
    {
        _shineSequence?.Kill();
        RestoreOriginalColors();
    }
}