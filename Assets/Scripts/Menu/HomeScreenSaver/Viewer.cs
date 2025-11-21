using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationController), typeof(ImageAnalyzer))]
public class Viewer : MonoBehaviour
{
    private float _delay;
    private bool _isTransitioning;
    private Sequence _transitionSequence;
    private ImageAnalyzer _imageAnalyzer;
    private AnimationController _animationController;
    private List<Sprite> _spriteSequence;

    private void Awake()
    {
        _imageAnalyzer = GetComponent<ImageAnalyzer>();
        _animationController = GetComponent<AnimationController>();
        _spriteSequence = new List<Sprite>();
        _isTransitioning = false;
        _delay = 0.1f;

        DOTween.Init(recycleAllByDefault: false, useSafeMode: true, LogBehaviour.Default);
        DOTween.SetTweensCapacity(4000, 1250);
    }

    private void Start()
    {
        ShowNextSprite();
    }

    private void OnEnable()
    {
        _animationController.ResumeAllAnimations();
    }

    private void OnDisable()
    {
        StopAllAnimations();
    }

    public void AddSprite(List<Sprite> sprites)
    {
        _spriteSequence = sprites;

        ShowNextSprite();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            if (gameObject.activeSelf) return;

            gameObject.SetActive(true);
            _animationController.ResumeAllAnimations();
            return;
        }

        _animationController.PauseAllAnimations();

        if (gameObject.activeSelf == false) return;
        gameObject.SetActive(false);
    }

    public void ShowNextSprite()
    {
        if (_isTransitioning || _spriteSequence.Count == 0) return;

        _isTransitioning = true;

        CreateTransitionSequence();

        int nextIndex = GetNextSpriteIndex();

        if (nextIndex >= 0 && nextIndex < _spriteSequence.Count)
        {
            _transitionSequence
                .AppendCallback(() => _imageAnalyzer.AnalyzeTexture(_spriteSequence[nextIndex]))
                .OnComplete(() => _isTransitioning = false);
        }
        else
        {
            _isTransitioning = false;
        }
    }

    private void CreateTransitionSequence()
    {
        DOTweenExtensions.SafeKill(_transitionSequence, true);
        _transitionSequence = DOTween.Sequence()
            .AppendInterval(_delay)
            .SetRecyclable(true);
    }

    private int GetNextSpriteIndex()
    {
        if (_spriteSequence.Count == 0) return -1;
        return Random.Range(0, _spriteSequence.Count);
    }

    private void StopAllAnimations()
    {
        _isTransitioning = false;
        DOTweenExtensions.SafeKill(_transitionSequence, true);
        _animationController.PauseAllAnimations();
        DOTween.Kill(this);
    }

    private void OnDestroy()
    {
        StopAllAnimations();
    }
}