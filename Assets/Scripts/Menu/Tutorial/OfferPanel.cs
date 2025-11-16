using System;
using UnityEngine;

[RequireComponent(typeof(Scaler), typeof(Voiceover))]
public class OfferPanel : MonoBehaviour
{
    [SerializeField] private MenuButtons _menuButtons;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _minScale = 0.2f;
    [SerializeField] private float _overshoot = 1.5f;
    [SerializeField] private float _delay = 0.1f;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _click;

    private Scaler _scaling;
    private Voiceover _voiceover;

    public event Action OnTutorial;
    public event Action OnCancelled;
    
    private void Awake()
    {
        _scaling = GetComponent<Scaler>();
        _voiceover = GetComponent<Voiceover>();
        _menuButtons.Initialize(LoadTutorial, DisablePanel);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _voiceover.Play(_audioClip);
        _scaling.TurnOn(Vector3.one, _duration, _delay, _overshoot);
        _menuButtons.ShowResumeButton();
    }

    private void OnDisable()
    {
        _scaling.SetInactive(_minScale);
    }

    private void LoadTutorial()
    {
        _voiceover.Play(_click);
        OnTutorial?.Invoke();
    }

    private void DisablePanel()
    {
        _voiceover.Play(_click);
        OnCancelled?.Invoke();
    }
}