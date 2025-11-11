using DG.Tweening;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    private float _duration;
    private float _amplitude;
    private int _quantityCycles;
    private Sequence _sequence;
    private Vector3 _initialRotation;

    private void Awake()
    {
        _amplitude = 10;
        _duration = 0.2f;
        _quantityCycles = 4;
    }

    public void Play()
    {
        _initialRotation = transform.eulerAngles;

        if (_sequence != null && _sequence.IsPlaying())
            return;

        Rotate();
    }

    public void Stop()
    {
        if (_sequence != null)
        {
            _sequence.Kill();

            transform.eulerAngles = _initialRotation;
        }
    }

    private void Rotate()
    {
        _sequence = DOTween.Sequence();

        CreateSequence(0, 0, _amplitude);

        CreateSequence(0, 0, -_amplitude);

        _sequence.SetLoops(_quantityCycles, LoopType.Restart).OnComplete(() => transform.eulerAngles = _initialRotation);

        _sequence.SetEase(Ease.Linear);
    }

    private void CreateSequence(float amplitudeX = 0, float amplitudeY = 0, float amplitudeZ = 0)
    {
        _sequence.Append(transform.DORotate(
           transform.eulerAngles + new Vector3(amplitudeX, amplitudeY, amplitudeZ),
           _duration)
           .SetEase(Ease.InSine));
    }

    private void OnDestroy()
    {
        if (_sequence != null)
            _sequence.Kill();
    }
}