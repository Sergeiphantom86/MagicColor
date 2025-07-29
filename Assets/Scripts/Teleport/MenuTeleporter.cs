using UnityEngine;
using UnityEngine.UI;

public class MenuTeleporter : Teleporter
{
    private const string ForestEmeraldGrove = nameof(ForestEmeraldGrove);

    [SerializeField] private Select _panel;
    [SerializeField] private Button _firstButton;
    [SerializeField] private Button _secondButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Transform _secondSpawnPoint;

    protected override void OnPlayerContact(Player player)
    {
        base.OnPlayerContact(player);

        ShowTeleportMenu();
    }

    private void ShowTeleportMenu()
    {
        _fadePanel.gameObject.SetActive(true);
        _panel.gameObject.SetActive(true);
        _fadePanel.RestoreValues(1, true);
        ButtonChoose();
    }

    private void ChooseLocation1()
    {
        StartCoroutine(TeleportPlayer(_defaultSpawnPoint.position));
        _panel.gameObject.SetActive(false);
    }

    private void ChooseLocation2()
    {
        StartCoroutine(TeleportPlayer(_secondSpawnPoint.position));
        _panel.gameObject.SetActive(false);
    }

    private void CloseMenu()
    {
        RegainControl();
        ButtonExit();
    }

    private void ButtonChoose()
    {
        _firstButton.onClick.AddListener(ChooseLocation1);
        _secondButton.onClick.AddListener(ChooseLocation2);
        _exitButton.onClick.AddListener(CloseMenu);
    }

    private void ButtonExit()
    {
        _firstButton.onClick.RemoveAllListeners();
        _secondButton.onClick.RemoveAllListeners();
        _exitButton.onClick.RemoveListener(CloseMenu);
    }
}