using System;
using UnityEngine;

[RequireComponent(typeof(Unblocker), typeof(Oscillator), typeof(Voiceover))]
public class Lock : MonoBehaviour
{
    [SerializeField] private AudioClip _flight;
    [SerializeField] private AudioClip _blocking;
    [SerializeField] private ErrorPanel _errorPanel;

    private Oscillator _ocillator;
    private Unblocker _unblocker;
    private Voiceover _voiceover;
    private LockPointer[] _lockPointers;
    private Collider _collider;
    private bool _isUsed;

    public bool IsUsed => _isUsed;

    public event Action OnUnblocking;

    private void Awake()
    {
        _ocillator = GetComponent<Oscillator>();
        _unblocker = GetComponent<Unblocker>();
        _voiceover = GetComponent<Voiceover>();
        _collider = GetComponent<Collider>();
        _lockPointers = GetComponentsInChildren<LockPointer>();

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

        if (_lockPointers == null)
        {
            Debug.LogError("LockPointer = null");
            return;
        }

        if (_voiceover == null)
        {
            Debug.LogError("Voiceover = null");
            return;
        }

        if (_collider == null)
        {
            Debug.LogError("Collider = null");
            return;
        }
    }

    public void SetUsed(bool isUsed)
    {
        _isUsed = isUsed;
    }

    public void SetAngle(Vector3 angleRotation)
    {
        transform.Rotate(angleRotation);
    }

    public void Unblock()
    {
        _unblocker.Play();
        _voiceover.PlayOneShot(_flight);
        _collider.enabled = false;
        SetColor();
        OnUnblocking?.Invoke();
    }

    public void ShakeUp()
    {
        if (_errorPanel != null)
        {
            _errorPanel.TurnOn();
        }

        _ocillator.Play();
        _voiceover.PlayOneShot(_blocking);
    }

    private void SetColor()
    {
        foreach (LockPointer lockPointer in _lockPointers)
        {
            lockPointer.SetColor();
        }
    }
}