using UnityEngine;
using DG.Tweening;

public class PageSlider : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _slideDuration = 0.3f;

    private int _currentPage;
    private int _pageCount;
    private float _pageWidth;

    public int CurrentPage => _currentPage;

    private void Awake()
    {
        _pageWidth = ((RectTransform)_content.parent).rect.width;
        _pageCount = _content.childCount;
    }

    public void SlideToPage(int page)
    {
        page = Mathf.Clamp(page, 0, _pageCount - 1);
        if (page == _currentPage) return;

        _currentPage = page;

        float targetX = -_pageWidth * _currentPage;
        _content.DOAnchorPosX(targetX, _slideDuration).SetEase(Ease.OutCubic);
    }

    public void Next()
    {
        SlideToPage(_currentPage + 1);
    }

    public void Prev()
    {
        SlideToPage(_currentPage - 1);
    }
}