using System.Collections;
using UnityEngine;
using YG;

public class NPCDialogueManager : MonoBehaviour
{
    [SerializeField] private Transform _triger;
    [SerializeField] private ConfirmationDialog _confirmationDialog;
    [SerializeField] private SelectionDialog _selectionDialog;
    [SerializeField] private CameraController _dialogueCameraController;
    [SerializeField] private GameSaveSystem _saveSystem;
    [SerializeField] private UISound _uISound;

    private NPC _nPC;
    private Player _player;
    private Rotator _rotator;
    private Prompter _prompter;
    private DialogueState _currentState;
    private string _npcKey;

    private enum DialogueState { Initial, HelpOptions, Completed }

    private void Awake()
    {
        _nPC = GetComponent<NPC>();
        _rotator = GetComponent<Rotator>();
        _prompter = GetComponent<Prompter>();

        _npcKey = $"NPC_{gameObject.name}_State";

        _prompter.turnOn();
    }

    private IEnumerator Start()
    {
        while (YG2.saves == null || _saveSystem == null)
        {
            yield return null;
        }

        _currentState = (DialogueState)_saveSystem.LoadNPCState(_npcKey, (int)DialogueState.Initial);
    }

    private void OnEnable()
    {
        _selectionDialog.OnRightButtonPressed += PressRightButton;
        _selectionDialog.OnLeftButtonPressed += PressLeftButton;
    }

    private void OnDisable()
    {
        _selectionDialog.OnRightButtonPressed -= PressRightButton;
        _selectionDialog.OnLeftButtonPressed -= PressLeftButton;
    }

    public void HandleConfirm()
    {
        switch (_currentState)
        {
            case DialogueState.Initial:
                ShowHelpOptions();
                break;

            case DialogueState.HelpOptions:
                _confirmationDialog.ShowIncomplete();
                break;

            default:
                DisableInteraction();
                break;
        }
    }

    public void HandleCancel()
    {
        switch (_currentState)
        {
            case DialogueState.Initial:
                DisableInteraction();
                break;

            case DialogueState.HelpOptions:
                DisableInteraction();
                break;

            default:
                _confirmationDialog.ShowGratitude();
                break;
        }
    }

    public void EnableInteraction(Player player)
    {
        _player = player;

        _player.TurnOffMovement();

        _rotator.StartRotationToPlayer(player.transform.position);
        
        _dialogueCameraController.StartDialogueView(_triger);

        StartCoroutine(fdhvudfv(1));
    }

    private void SaveCurrentState()
    {
        if (_saveSystem != null)
        {
            _saveSystem.SaveNPCState(_npcKey, (int)_currentState);
        }
    }

    private void SelectTask(string desiredSprite)
    {
        _selectionDialog.Hide();

        _nPC.GetSelectedSprite(desiredSprite);
        _player.UpdateSprite(_nPC.Sprite);

        RegainControl();
        SaveCurrentState();
    }

    private void RegainControl()
    {
        _dialogueCameraController.EndDialogueView();
        _player.TurnOnMovement();
    }

    private void ShowHelpOptions()
    {
        _confirmationDialog.gameObject.SetActive(false);
        _confirmationDialog.Hide();
        _prompter.turnOff();
        _selectionDialog.Show();
        StartCoroutine(fdhvudfv(0.3f));
        _currentState = DialogueState.HelpOptions;

        SaveCurrentState();
    }

    public void DisableInteraction()
    {
        RegainControl();
        _confirmationDialog.Hide();
    }

    private void PressLeftButton()
    {
        SelectTask("Left");
    }

    private void PressRightButton()
    {
        SelectTask("Right");
    }

    private IEnumerator fdhvudfv(float dalay)
    {
        yield return new WaitForSeconds(dalay);

        _uISound.OnPanelAnimation();
    }
}