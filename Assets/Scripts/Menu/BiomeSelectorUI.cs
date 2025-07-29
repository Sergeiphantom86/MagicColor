using UnityEngine;
using UnityEngine.UI;
using System;
using static SceneLoader;

[RequireComponent(typeof(Animator))]
public class BiomeSelectorUI : MonoBehaviour
{
    private const string EnchantedForestArcanumGlade = nameof(EnchantedForestArcanumGlade);
    private const string SwampMurkwoodBog = nameof(SwampMurkwoodBog);
    private const string IsClose = nameof(IsClose);

    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _enchantedForestButton;
    [SerializeField] private Button _swampButton;
    [SerializeField] private Animator _animator;
    [SerializeField] private Select _panel;

    private void OnEnable()
    {
       
        _exitButton.onClick.AddListener(HandleExit);
    }

    public void ShowPanel(bool isOn)
    {
        _panel.gameObject.SetActive(isOn);
    }

    private void HidePanel()
    {
        _animator.SetBool(IsClose, true);
    }

    private void HandleExit()
    {
        HidePanel();
    }
}