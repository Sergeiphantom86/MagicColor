using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Collider), typeof(InteractionHandler))]
public class DapterAssembly : MonoBehaviour
{
    private const string PuzzleScene = nameof(PuzzleScene);
    private const string IsClose = nameof(IsClose);

    [SerializeField] private Animator _animator;
    [SerializeField] private UISound _uISound;
    [SerializeField] private Select _panel;
    [SerializeField] private Button _swampButton;
    [SerializeField] private Button _exit;

    private InteractionHandler _interactionHandler;
    private Collider _collider;
    private Player _player;
    private Altar _altar;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _interactionHandler = GetComponent<InteractionHandler>();
        _altar = GetComponent<Altar>();

        _collider.isTrigger = true;
    }

    private void OnEnable()
    {
        _interactionHandler.OnContact += Interact;
    }

    private void OnDisable()
    {
        _interactionHandler.OnContact -= Interact;
    }

    private void Interact(Player player)
    {
        _player = player;

        _player.TurnOffMovement();
        _uISound.OnPanelAnimation();
        SelectPanelDisplay();
        ButtonChoose();
        ChoosePath(true);
    }

    private void SelectPanelDisplay()
    {
        if (_player.GetSprite() == null)
        {
            _panel.ShowWithoutButton();
            return;
        }

        _panel.ShowWithButton();
    }

    private void ChooseSwampHandler()
    {
        _uISound.OnClick();
        Choose();
    }

    private void ChoosePath()
    {
        _uISound.OnClick();
        ChoosePath(false);
    }

    private void ChoosePath(bool isOn)
    {
        _panel.gameObject.SetActive(isOn);

        if (isOn == false)
        {
            ButtonExit();
            _player.TurnOnMovement();
        }
    }

    private void Choose()
    {
        if (_animator != null)
        {
            _animator.SetBool(IsClose, true);
        }

        _altar.GoToAnotherScene(_player);
    }

    private void ButtonChoose()
    {
        _swampButton.onClick.AddListener(ChooseSwampHandler);
        _exit.onClick.AddListener(ChoosePath);
    }

    private void ButtonExit()
    {
        _swampButton.onClick.RemoveListener(ChooseSwampHandler);
        _exit.onClick.RemoveListener(ChoosePath);
    }
}