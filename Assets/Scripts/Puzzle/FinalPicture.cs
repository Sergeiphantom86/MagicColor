using DG.Tweening;
using UnityEngine;

public class FinalPicture : MonoBehaviour
{
    [SerializeField] private float _moveYDuration;
    [SerializeField] private float _targetYPosition;
    [SerializeField] private float _scaleDuration;

    private Vector3 _targetScale;

    private void Awake()
    {
        _targetScale = Vector3.one * 2;
}

    public void Demonstrate(RectTransform canvasRect)
    {
        MoveY();
        Increase();
    }

    private void MoveY()
    {
        transform.DOLocalMoveY(_targetYPosition, _moveYDuration)
                .SetEase(Ease.OutBack);
    }

    private void Increase()
    {
        transform.DOScale(_targetScale, _scaleDuration)
                .SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}