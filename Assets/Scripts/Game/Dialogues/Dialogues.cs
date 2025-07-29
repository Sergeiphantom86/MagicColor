using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(DialogueAnimator))]
public class Dialogues : MonoBehaviour
{
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _confirmButton;

    public event Action OnLeftButtonPressed;
    public event Action OnRightButtonPressed;
    private DialogueAnimator _dialogueAnimator;

    private void Awake()
    {
        _dialogueAnimator = GetComponent<DialogueAnimator>();
    }

    private void OnEnable()
    {
        _cancelButton.onClick.AddListener(HandleCancel);
        _confirmButton.onClick.AddListener(HandleConfirm);
    }

    private void OnDestroy()
    {
        _cancelButton.onClick.RemoveListener(HandleCancel);
        _confirmButton.onClick.RemoveListener(HandleConfirm);
    }

    private void HandleCancel()
    {
        OnRightButtonPressed?.Invoke();
    }

    private void HandleConfirm()
    {
        OnLeftButtonPressed?.Invoke();
    }

    public void Hide()
    {
        _dialogueAnimator.Hide();
    }

    public void Show()
    {
        _dialogueAnimator.Show();
    }

    public void ShowGratitude()
    {
        Show();

        _cancelButton.gameObject.SetActive(false);
        _confirmButton.gameObject.SetActive(true);
    }
    public void ShowIncomplete()
    {
        Show();

        _confirmButton.gameObject.SetActive(false);
    }
}