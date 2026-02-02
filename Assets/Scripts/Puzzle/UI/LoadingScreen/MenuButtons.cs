using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class MenuButtons
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _resumeButton;

    public void Initialize(Action onStart, Action onResume)
    {
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(() => onStart?.Invoke());

            _startButton.gameObject.SetActive(false);
        }

        if (_resumeButton != null)
        {
            _resumeButton.onClick.AddListener(() => onResume?.Invoke());

            _resumeButton.gameObject.SetActive(false);
        }
    }

    public void CleanUp()
    {
        if (_startButton != null)
        {
            _startButton.onClick.RemoveAllListeners();
        }

        if (_resumeButton != null)
        {
            _resumeButton.onClick.RemoveAllListeners();
        }
    }

    public void ShowResumeButton()
    {
        if (_resumeButton != null)
        {
            _resumeButton.gameObject.SetActive(true);
        }
    }

    public void ShowStartButton()
    {
        if (_startButton != null)
        {
            _startButton.gameObject.SetActive(true);
        }
    }

    public void HideStartButton() => 
        _startButton.gameObject.SetActive(false);
}