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
        _startButton.onClick.AddListener(() => onStart?.Invoke());
        _resumeButton.onClick.AddListener(() => onResume?.Invoke());
        _resumeButton.gameObject.SetActive(false);
    }

    public void CleanUp()
    {
        _startButton.onClick.RemoveAllListeners();
        _resumeButton.onClick.RemoveAllListeners();
    }

    public void ShowResumeButton() => 
        _resumeButton.gameObject.SetActive(true);

    public void HideStartButton() => 
        _startButton.gameObject.SetActive(false);
}