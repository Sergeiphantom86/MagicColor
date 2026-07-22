using Game;
using Menu.Interaction.Abilitys;
using Menu.Shop;
using PuzzleResources.MovingBlocks;
using TMPro;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle.TutorialStateMachine
{
    public class TutorialAbilitiesHandler : MonoBehaviour
    {
        [SerializeField] private Hints _hintsUI;
        [SerializeField] private AbilityButton _bagAbilities;
        [SerializeField] private CloseGameButton _closeGame;
        [SerializeField] private PurchaseButton _purchaseButton;
        [SerializeField] private HandMover _handMoverUI;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private PauseButton _pauseButton;
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private StateMachine _tutorialStateMachine;

        private TutorialAbilities _tutorialAbilities;

        public TutorialAbilities Abilities => _tutorialAbilities;

        public void SetContext(TutorialContext tutorialContext)
        {
            _tutorialAbilities = new(
                   _tutorialStateMachine,
                   tutorialContext,
                   _inputHandler,
                   _pauseButton,
                   _priceText,
                   _handMoverUI,
                   _purchaseButton,
                   _closeGame,
                   _bagAbilities,
                   _hintsUI);
        }
    }
}