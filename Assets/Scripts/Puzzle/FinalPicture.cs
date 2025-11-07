using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(IUIAnimator))]
public class FinalPicture : MonoBehaviour
{
    private IUIAnimator _animator;
    private float _positionZ;

    private void Awake()
    {
        _positionZ = -50;
        _animator = GetComponent<IUIAnimator>();
    }

    public void SetPositionZ()
    {
        Vector3 position = transform.position;
        position.z = _positionZ;
        transform.localPosition = position;
    }

    public Sequence Move(RectTransform canvasRect)
    {
        return _animator.Move(canvasRect);
    }

    public void Increase()
    {
        _animator.Increase();
    }
}