using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private WalletAnimator _walletAnimator;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private Blocker _blocker;

    private Voiceover _voiceover;
    private Button _button;
    private IProgressSaver _progressSaver;
    private IActivatable _activatable;

    public Button Button => _button;

    public event Action OnClick;
    public event Action<long> OnPurchased;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _voiceover = GetComponent<Voiceover>();
        _progressSaver = new ProgressSaver();
        _button.interactable = true;

        _activatable = _blocker;
        _activatable.Deactivate();

        if (_blocker != null && _progressSaver.Saves.IsUnlockAbilities == false)
        {
            _activatable.Activate();
            _button.interactable = false;
        }
    }

    private void Start()
    {
        _button.onClick.AddListener(Buy);
    }

    private void OnEnable()
    {
        _walletAnimator.Finished += Wait;
    }

    private void OnDisable()
    {
        _walletAnimator.Finished -= Wait;
    }

    public void Click()
    {
        OnClick?.Invoke();

        _button.interactable = false;

        _voiceover.PlayOneShot(_audioClip);

    }

    private void Buy()
    {
        if (long.TryParse(_textMeshProUGUI.text, out long result) == false) return;

        OnPurchased?.Invoke(result);
    }

    private void Wait()
    {
        _button.interactable = true;
    }
}