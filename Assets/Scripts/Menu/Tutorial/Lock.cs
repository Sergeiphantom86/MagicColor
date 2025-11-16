using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(Unblocker), typeof(Oscillator), typeof(Voiceover))]
public class Lock : MonoBehaviour
{
    [SerializeField] private AudioClip _flight;
    [SerializeField] private AudioClip _blocking;
    [SerializeField] private ErrorPanel _errorPanel;

    private Sequence _movementSequence;
    private Oscillator _ocillator;
    private Unblocker _unblocker;
    private Voiceover _voiceover;

    public event Action OnUnblocking;

    private void Awake()
    {
        _ocillator = GetComponent<Oscillator>();
        _unblocker = GetComponent<Unblocker>();
        _voiceover = GetComponent<Voiceover>();

        if (_ocillator == null)
        {
            Debug.LogError("Oscillator = null");
            return;
        }

        if (_unblocker == null)
        {
            Debug.LogError("Unblocker = null");
            return;
        }

        if (_voiceover == null)
        {
            Debug.LogError("Voiceover = null");
            return;
        }
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
        _voiceover.Play(_flight);

        OnUnblocking?.Invoke();
    }

    public void ShakeUp()
    {
        if (_errorPanel != null)
        {
            _errorPanel.TurnOn();
        }

        _ocillator.Play();
        _voiceover.Play(_blocking);
    }

    private void OnDestroy()
    {
        _movementSequence?.Kill(_blocking);
    }
}