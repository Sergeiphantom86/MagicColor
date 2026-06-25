using DG.Tweening;
using UnityEngine;
public interface IUIAnimator
{
    public Sequence Move(RectTransform canvasRect);

    public void Return(RectTransform canvasRect);

    public void Increase();
}