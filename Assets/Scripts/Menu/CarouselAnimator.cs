using DG.Tweening;
using UnityEngine;

public class CarouselAnimator
{
    private readonly float _duration;

    public CarouselAnimator(float duration)
    {
        _duration = duration;
    }

    public void ApplyImmediate(RectTransform rect, CanvasGroup canvas, float positionX, float scale, float alpha)
    {
        rect.anchoredPosition = new Vector2(positionX, rect.anchoredPosition.y);
        rect.localScale = Vector3.one * scale;
        canvas.alpha = alpha;
    }

    public void ApplyAnimated(RectTransform rect, CanvasGroup canvas, float positionX, float scale, float alpha)
    {
        rect.DOKill();
        canvas.DOKill();

        rect.DOAnchorPosX(positionX, _duration).SetEase(Ease.OutBack);
        rect.DOScale(scale, _duration).SetEase(Ease.OutBack);
        canvas.DOFade(alpha, _duration);
    }
}