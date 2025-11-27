using DG.Tweening;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    private IUIAnimator _animator;

    private void Awake()
    {
        _animator = GetComponent<IUIAnimator>();
    }

    public Sequence Move(RectTransform canvasRect)
    {
        canvasRect = _canvas.GetComponent<RectTransform>();

        return _animator.Move(canvasRect);
    }

    public void Return(RectTransform canvasRect)
    {
        canvasRect = _canvas.GetComponent<RectTransform>();

        _animator.Return(canvasRect);
    }
}