using TMPro;
using UnityEngine;

public class TutorialAbilities : TutorialStater
{
    private const string Price = "3000";
    private const string TemporaryPrice = "10";

    private readonly Hints _hintsUI;
    private readonly IInputHandler _input;
    private readonly TextMeshProUGUI _text;
    private readonly HandMover _handMoverUI; 
    private readonly TutorialContext _context;
    private readonly PauseButton _pauseButton;
    private readonly CloseGameButton _closeGame; 
    private readonly AbilityButton _bagAbilities; 
    private readonly PurchaseButton _purchaseButton; 
    private readonly TutorialStateMachine _stateMachine;
    private readonly IProgressSaver _progressSaver;

    public TutorialAbilities(TutorialStateMachine stateMachine, TutorialContext context, IInputHandler input, PauseButton pauseButton, TextMeshProUGUI text, HandMover handMoverUI, PurchaseButton purchaseButton, CloseGameButton closeGame, AbilityButton bagAbilities, Hints hintsUI) : base(stateMachine, context)
    {
        _input = input;
        _text = text;
        _hintsUI = hintsUI;
        _context = context;
        _closeGame = closeGame;
        _pauseButton = pauseButton;
        _handMoverUI = handMoverUI;
        _stateMachine = stateMachine;
        _bagAbilities = bagAbilities;
        _purchaseButton = purchaseButton;
        _progressSaver = new ProgressSaver();
       
    }

    public override void Enter()
    {
        _progressSaver.SetAbilityTutorial();

        MoveTarget(_pauseButton.transform.position);

        _text.text = TemporaryPrice;

        _pauseButton.Button.onClick.AddListener(Move);
    }

    public override void Exit()
    {
        _pauseButton.Button.onClick.RemoveListener(Move);
        _purchaseButton.Button.onClick.RemoveListener(CloseMenu);
        _closeGame.Button.onClick.RemoveListener(GoToUse);
        _bagAbilities.Button.onClick.RemoveListener(UseAbility);

        _input.OnSelected -= Apply;

        _handMoverUI.Stop();
        _handMoverUI.gameObject.SetActive(false);
        _hintsUI.gameObject.SetActive(false);
    }

    private void MoveTarget(Vector3 position)
    {
        _handMoverUI.Stop();
        _handMoverUI.transform.position = position;
        _handMoverUI.EnableScaleAnimation();
    }

    private void Move()
    {
        MoveTarget(_purchaseButton.transform.position);

        _text.text = Price;

        _purchaseButton.Button.onClick.AddListener(CloseMenu);
    }

    private void CloseMenu()
    {
        MoveTarget(_closeGame.transform.position);

        _closeGame.Button.onClick.AddListener(GoToUse);
    }

    private void GoToUse()
    {
        MoveTarget(_bagAbilities.transform.position);

        _bagAbilities.Button.onClick.AddListener(UseAbility);
    }

    private void UseAbility()
    {
        _handMoverUI.gameObject.SetActive(false);
        _hintsUI.gameObject.SetActive(true);
        _input.OnSelected += Apply;
    }

    private void Apply(Vector2 position)
    {
        _stateMachine.ChangeState(new CompletionState(_stateMachine, _context));
    }
}