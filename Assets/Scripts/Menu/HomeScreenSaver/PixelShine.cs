using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PuzzleEditor;
using UnityEngine;

namespace Menu.HomeScreenSaver
{
    [RequireComponent(typeof(AppearanceAnimator))]
    public class PixelShine : MonoBehaviour, IAnimatable
    {
        [SerializeField]
        private ParticleSystem _particleSystem;

        private float _shineDuration;
        private float _pauseBetweenPasses;
        private float _delayBetweenPixels;
        private Sequence _shineSequence;
        private List<Fragment> _validFragments;
        private AppearanceAnimator _appearanceAnimator;
        private Dictionary<Fragment, Color> _originalColors;

        public event Action<List<Fragment>> Glistened;

        private void Awake()
        {
            _shineDuration = 0.02f;
            _pauseBetweenPasses = 0.5f;
            _delayBetweenPixels = 0.001f;
            _appearanceAnimator = GetComponent<AppearanceAnimator>();

            _validFragments = new List<Fragment>();
            _originalColors = new Dictionary<Fragment, Color>();
        }

        private void OnEnable()
        {
            _appearanceAnimator.AppearanceCompleted += StartShineAnimation;
        }

        private void OnDisable()
        {
            _appearanceAnimator.AppearanceCompleted -= StartShineAnimation;
        }

        public void PauseAnimations() => DOTweenExtensions.SafePause(_shineSequence);

        public void ResumeAnimations() => DOTweenExtensions.SafePlay(_shineSequence);

        private void StartShineAnimation()
        {
            if (_appearanceAnimator.Fragments.Count == 0)
                return;

            _validFragments = _appearanceAnimator
                .Fragments.Where(f => f != null && f.Renderer != null)
                .ToList();

            if (_validFragments.Count == 0)
                return;

            CreateShineSequence();
            StoreOriginalColors();

            _shineSequence.OnComplete(() =>
            {
                RestoreOriginalColors();

                Glistened?.Invoke(_validFragments);
            });

            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Play();
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
                if (pair.Key.Renderer != null)
                    pair.Key.Renderer.color = pair.Value;
            }

            _originalColors.Clear();
        }

        private void CreateShineSequence()
        {
            ResetAnimation();

            _shineSequence = DOTween.Sequence();

            for (int i = 0; i < _validFragments.Count; i++)
            {
                _shineSequence.Insert(
                    i * _delayBetweenPixels,
                    CreateShineTween(_validFragments[i])
                );
            }

            _shineSequence.AppendInterval(_pauseBetweenPasses);
        }

        private Sequence CreateShineTween(Fragment fragment)
        {
            SpriteRenderer renderer = fragment.Renderer;

            return DOTween
                .Sequence()
                .Append(
                    renderer
                        .DOColor(Color.white + Color.yellow, _shineDuration)
                        .SetEase(Ease.OutQuad)
                )
                .Append(renderer.DOColor(renderer.color, _shineDuration).SetEase(Ease.InQuad));
        }

        private void ResetAnimation()
        {
            DOTweenExtensions.SafeKill(_shineSequence);

            if (_validFragments != null)
            {
                foreach (var fragment in _validFragments)
                {
                    if (fragment != null)
                        fragment.transform.DOKill();
                }
            }

            _shineSequence = null;
        }
    }
}