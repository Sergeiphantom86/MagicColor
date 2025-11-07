using DG.Tweening;
using UnityEngine;

public class Pen : MonoBehaviour
{
    private IUIAnimator _animator;

    private void Awake()
    {
        _animator = GetComponent<IUIAnimator>();
    }

    public Sequence Move(RectTransform canvasRect)
    {
        return _animator.Move(canvasRect);
    }

    public void Return(RectTransform canvasRect)
    {
        _animator.Return(canvasRect);
    }
}