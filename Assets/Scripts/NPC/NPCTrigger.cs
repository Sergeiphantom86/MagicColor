using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(NPC), typeof(InteractionHandler), typeof(NPCDialogueManager))]
public class NPCTrigger : MonoBehaviour
{
    [SerializeField] private UISound _uISound;
    [SerializeField] private Transform _triger;
    [SerializeField] private ConfirmationDialog _dialogues;
    [SerializeField] private GameSaveSystem _gameSaveSystem;
    [SerializeField] private CameraController _dialogueCameraController;

    private InteractionHandler _interactionHandler;
    private NPCDialogueManager _dialogueManager;

    public Action OnPlayerEnter;
    public Action OnPlayerExit;

    private void Awake()
    {
        _interactionHandler = GetComponent<InteractionHandler>();
        _dialogueManager = GetComponent<NPCDialogueManager>();
    }

    private void OnEnable()
    {
        _dialogues.OnLeftButtonPressed += PressButtonConfirm;
        _dialogues.OnRightButtonPressed += PressButtonCancel;

        _interactionHandler.OnContact += ShowWindows;
    }

    private void OnDisable()
    {
        _dialogues.OnLeftButtonPressed -= PressButtonConfirm;
        _dialogues.OnRightButtonPressed -= PressButtonCancel;

        _interactionHandler.OnContact -= ShowWindows;

        if (DOTween.IsTweening(transform))
        {
            DOTween.Kill(transform, true);
        }
    }

    private void ShowWindows(Player player)
    {
        _dialogueManager.EnableInteraction(player);
    }

    private void PressButtonConfirm()
    {
        _uISound.OnClick();
        _dialogueManager.HandleConfirm();
    }

    private void PressButtonCancel()
    {
        _uISound.OnClick();
        _dialogueManager.HandleCancel();
    }
}