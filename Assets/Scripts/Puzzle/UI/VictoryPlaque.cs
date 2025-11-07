using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(IUIAnimator))]
public class VictoryPlaque : MonoBehaviour
{
    private IUIAnimator animator;

    private void Awake()
    {
        animator = GetComponent<IUIAnimator>();
    }

    public Sequence Move(RectTransform canvasRect)
    {
        return animator.Move(canvasRect);
    }
}