using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Button))]
public class RewardAdForSpins : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Counter _counter;

    private string _rewardID = "add_spins";
    private Button _button;

    public event Action OnSpinsAdded;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (_button == null)
        {
            Debug.LogError("Button не назначен!!!");
            return;
        }

        if (_counter == null)
        {
            Debug.LogError("Counter не назначен!!!");
            return;
        }

        _button.onClick.AddListener(ShowRewardedAd);

        YG2.onRewardAdv += OnRewardReceived;
        YG2.onOpenRewardedAdv += OnAdOpened;
        YG2.onCloseRewardedAdv += OnAdClosed;
        YG2.onErrorRewardedAdv += OnAdError;
    }

    private void ShowRewardedAd()
    {
        _button.interactable = false;
        YG2.RewardedAdvShow(_rewardID);
    }

    private void OnRewardReceived(string id)
    {
        if (id == _rewardID && _counter != null)
        {
            for (int i = 0; i < ParseTextToInt(); i++)
            {
                OnSpinsAdded?.Invoke();
            }
        }
    }

    public int ParseTextToInt()
    {
        string numericText = new string(_textMeshPro.text
            .Where(c => char.IsDigit(c) || c == '-')
            .ToArray());

        return int.TryParse(numericText, out int result) ? result : 0;
    }

    private void OnAdOpened()
    {
        Time.timeScale = 0f;
    }

    private void OnAdClosed()
    {
        Time.timeScale = 1f;
    }

    private void OnAdError()
    {
        _button.interactable = true;
    }

    private void OnDestroy()
    {
        YG2.onRewardAdv -= OnRewardReceived;
        YG2.onOpenRewardedAdv -= OnAdOpened;
        YG2.onCloseRewardedAdv -= OnAdClosed;
        YG2.onErrorRewardedAdv -= OnAdError;

        if (_button != null)
            _button.onClick.RemoveListener(ShowRewardedAd);
    }
}