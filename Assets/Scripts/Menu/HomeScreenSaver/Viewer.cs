using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationController), typeof(ImageAnalyzer))]
public class Viewer : MonoBehaviour
{
    [SerializeField] private List<Sprite> _spriteSequence;

    private ImageAnalyzer _imageAnalyzer;
    private AnimationController _animationController;
    private int _currentSpriteIndex;
    private bool _isTransitioning;

    private void Awake()
    {
        _imageAnalyzer = GetComponent<ImageAnalyzer>();
        _animationController = GetComponent<AnimationController>();
        _currentSpriteIndex = -1;
        _isTransitioning = false;
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
        _animationController.PauseAllAnimations();
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
        
        int nextIndex = GetNextSpriteIndex();

        if (nextIndex >= 0 && nextIndex < _spriteSequence.Count)
        {
            _imageAnalyzer.AnalyzeTexture(_spriteSequence[nextIndex]);
        }

        _isTransitioning = false;
    }

    private int GetNextSpriteIndex()
    {
        if (_spriteSequence.Count == 0) return -1;

        _currentSpriteIndex = Random.Range(0, _spriteSequence.Count);
        return _currentSpriteIndex;
    }
}