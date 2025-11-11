using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Unblocker), typeof(Oscillator))]
public class Lock : MonoBehaviour
{
    private Sequence _movementSequence;
    private Oscillator _ocillator;
    private Unblocker _unblocker;

    private void Awake()
    {
        _ocillator = GetComponent<Oscillator>();
        _unblocker = GetComponent<Unblocker>();
    }

    public void SetAngle(Vector3 angleRotation)
    {
        if (_movementSequence == null || _movementSequence.IsActive() == false)
        {
            _movementSequence = DOTween.Sequence();
            _movementSequence.SetLink(gameObject);
        }

        _movementSequence.Append(transform.DORotate(angleRotation, 0));
    }

    public void Unblock()
    {
        _unblocker.Play();
    }

    public void ShakeUp()
    {
        _ocillator.Play();
    }

    private void OnDestroy()
    {
        _movementSequence?.Kill();
    }
}