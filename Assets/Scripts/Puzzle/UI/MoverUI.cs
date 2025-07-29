using DG.Tweening;
using UnityEngine;

public class MoverUI
{
    Vector3 _targetPosition;
    Vector3 _targetScale;

    public Sequence EnableMotionAnimation(Transform transform, float duration, Camera camera, Sequence sequence, float positionScreenX,float positionScreenY, float positionScreenZ = 0)
    {
        if (sequence == null)
        {
            sequence = DOTween.Sequence();
        }

        _targetPosition = camera.ViewportToWorldPoint(new Vector3(positionScreenX, positionScreenY, positionScreenZ));

        return sequence.Join(transform.DOMove(_targetPosition, duration));
    }

    public void EnableAnimationResizing(Transform transform, float duration, Sequence sequence, float scale = 0)
    {
        if (sequence == null)
        {
            sequence = DOTween.Sequence();
        }

        _targetScale = transform.localScale * scale;

        sequence.Join(transform.DOScale(_targetScale, duration).SetEase(Ease.OutBack));
    }
}
