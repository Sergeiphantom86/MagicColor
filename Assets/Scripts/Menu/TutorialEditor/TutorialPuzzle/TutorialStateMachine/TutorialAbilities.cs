using Game;
using Menu.ButtonEditor.Ability;
using Menu.Shop;
using PuzzleEditor.MovingBlocks;
using TMPro;
using UnityEngine;
using YG;

namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{
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
        private readonly StateMachine _stateMachine;

        public TutorialAbilities(StateMachine stateMachine,
        TutorialContext context,
        IInputHandler input,
        PauseButton pauseButton,
        TextMeshProUGUI text,
        HandMover handMoverUI,
        PurchaseButton purchaseButton,
        CloseGameButton closeGame,
        AbilityButton bagAbilities,
        Hints hintsUI)
        : base(stateMachine, context)
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
        }

        public override void Enter()
        {
            YG2.saves.IsAbilityTutorial = true;

            MoveTarget(_pauseButton.transform.position);

            _text.text = TemporaryPrice;

            _pauseButton.Button.onClick.AddListener(OnMove);
        }

        public override void Exit()
        {
            _pauseButton.Button.onClick.RemoveListener(OnMove);
            _purchaseButton.Button.onClick.RemoveListener(OnCloseMenu);
            _closeGame.Button.onClick.RemoveListener(OnGoToUse);
            _bagAbilities.Button.onClick.RemoveListener(OnUseAbility);

            _input.Selected -= OnApply;

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

        private void OnMove()
        {
            MoveTarget(_purchaseButton.transform.position);

            _text.text = Price;

            _purchaseButton.Button.onClick.AddListener(OnCloseMenu);
        }

        private void OnCloseMenu()
        {
            MoveTarget(_closeGame.transform.position);

            _closeGame.Button.onClick.AddListener(OnGoToUse);
        }

        private void OnGoToUse()
        {
            MoveTarget(_bagAbilities.transform.position);

            _bagAbilities.Button.onClick.AddListener(OnUseAbility);
        }

        private void OnUseAbility()
        {
            _handMoverUI.gameObject.SetActive(false);
            _hintsUI.gameObject.SetActive(true);
            _input.Selected += OnApply;
        }

        private void OnApply(Vector2 position)
        {
            _stateMachine.ChangeState(new CompletionState(_stateMachine, _context));
        }
    }
}