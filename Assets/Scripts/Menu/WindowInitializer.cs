using System;
using System.Collections.Generic;
using UnityEngine;

public class WindowInitializer : MonoBehaviour
{  
    [SerializeField] private Window _gameSelection;
    [SerializeField] private Window _shopWindow;
    [SerializeField] private Window _settingsWindow;
    [SerializeField] private Window _leaderboardWindow;

    private Dictionary<string, Action> _windowActions;
    private bool _isInitialized;

    public Dictionary<string, Action> WindowActions => _windowActions;

    public void Initialize()
    {
        if (_isInitialized) return;

        _windowActions = new Dictionary<string, Action>();
        RegisterActions();
        _isInitialized = true;
    }

    private void RegisterActions()
    {
        RegisterAction(ButtonFactory.Play, () => ToggleWindow(_gameSelection));
        RegisterAction(ButtonFactory.Shop, () => ToggleWindow(_shopWindow));
        RegisterAction(ButtonFactory.Settings, () => ToggleWindow(_settingsWindow));
        RegisterAction(ButtonFactory.Leaderboard, () => ToggleWindow(_leaderboardWindow));
    }

    public void RegisterAction(string actionName, Action action)
    {
        if (_windowActions.ContainsKey(actionName))
        {
            Debug.LogWarning($"Action '{actionName}' is already registered");
            return;
        }

        _windowActions.Add(actionName, action);
    }

    private void ToggleWindow(Window window)
    {
        if (window == null)
        {
            Debug.LogError("Trying to toggle null window!");
            return;
        }

        window.Toggle();
    }
}