using DG.Tweening;
using UnityEngine;

public class LoadingRotator : MonoBehaviour
{
    private float _rotationSpeed;
    private bool _clockwise;
    private int _tweenersCapacity;
    private int _sequencesCapacity;

    private void Awake()
    {
        _rotationSpeed = 90f;
        _tweenersCapacity = 10000;
        _sequencesCapacity = 1250;
        _clockwise = true;

        DOTween.Init(recycleAllByDefault: false, useSafeMode: true, LogBehaviour.Default);
        DOTween.SetTweensCapacity(tweenersCapacity: _tweenersCapacity, sequencesCapacity: _sequencesCapacity);
    }

    private void Update()
    {
        float direction = _clockwise ? -1f : 1f;

        transform.Rotate(Vector3.back, _rotationSpeed * direction * Time.deltaTime);
    }
}