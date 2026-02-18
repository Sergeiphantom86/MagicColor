using System;
using UnityEngine;

[RequireComponent(typeof(Scaler), typeof(Voiceover))]
public class OfferPanel : MonoBehaviour
{
    [SerializeField] private MenuButtons _menuButtons;
    [SerializeField] private Rewards _rewards;
    [SerializeField] private float _duration;
    [SerializeField] private float _minScale;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private float _overshoot;
    [SerializeField] private float _delay;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _click;

    private Vector3 _scale;
    private Scaler _scaling;
    private Voiceover _voiceover;

    public event Action OnConsent;
    public event Action OnCancelled;

    private void Awake()
    {
        _scale = Vector3.one * _scaleMultiplier;

        _scaling = GetComponent<Scaler>();
        _voiceover = GetComponent<Voiceover>();

        _menuButtons.Initialize(Confirm, Refuse);

        TurnOff();
    }

    private void OnEnable()
    {
        TurnOnSound(_audioClip);

        _scaling.ChangeSize(_scale, _duration, _delay, _overshoot);

        _menuButtons.ShowResumeButton();
        _menuButtons.ShowStartButton();
    }

    private void OnDisable()
    {
        _scaling.SetInactive(_minScale);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    private void Confirm()
    {
        if (_rewards != null)
        {
            _rewards.Save();
        }

        TurnOnSound(_click);

        OnConsent?.Invoke();

        TurnOff();
    }

    private void Refuse()
    {
        TurnOnSound(_click);

        OnCancelled?.Invoke();

        TurnOff();
    }

    private void TurnOnSound(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            _voiceover.PlayOneShot(audioClip);
        }
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}