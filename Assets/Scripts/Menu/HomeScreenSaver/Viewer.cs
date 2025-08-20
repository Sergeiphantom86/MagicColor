using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationController), typeof(ImageAnalyzer))]
public class Viewer : MonoBehaviour
{
    [Header("Sprite Sequence")]
    [SerializeField] private List<Sprite> _spriteSequence;

    private ImageAnalyzer _imageAnalyzer;
    private AnimationController _animationController;
    private int _currentSpriteIndex = -1;
    private bool _isTransitioning = false;

    private void Awake()
    {
        _imageAnalyzer = GetComponent<ImageAnalyzer>();
        _animationController = GetComponent<AnimationController>();
    }

    private void Start()
    {
        ShowNextSprite();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            _animationController.ResumeAllAnimations();
        }
        else
        {
            _animationController.PauseAllAnimations();

            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
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
        _currentSpriteIndex = (_currentSpriteIndex + 1) % _spriteSequence.Count;
        return _currentSpriteIndex;
    }

    private void OnEnable()
    {
        _animationController?.ResumeAllAnimations();
    }

    private void OnDisable()
    {
        _animationController?.PauseAllAnimations();
    }
}