using DG.Tweening;
using UnityEngine;

public class LoadingRotator : MonoBehaviour
{
    private float _rotationSpeed;
    private bool _clockwise;

    private void Awake()
    {
        _rotationSpeed = 90f;
        _clockwise = true;

        DOTween.Init(recycleAllByDefault: false, useSafeMode: true, LogBehaviour.Default);
        DOTween.SetTweensCapacity(4000, 1250);
    }

    private void Update()
    {
        float direction = _clockwise ? -1f : 1f;

        transform.Rotate(Vector3.back, _rotationSpeed * direction * Time.deltaTime);
    }
}