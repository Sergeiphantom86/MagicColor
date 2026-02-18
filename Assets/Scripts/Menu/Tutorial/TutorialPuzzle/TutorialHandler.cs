using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TutorialHandler : TutorialStateMachine
{
    [SerializeField] private Key _key;
    [SerializeField] private Lock _lock;
    [SerializeField] private Hints _hints;
    [SerializeField] private Hints _hintsUI;
    [SerializeField] private Timer _timer;
    [SerializeField] private Rotator _rotator;
    [SerializeField] private BlockSpawner _container;
    [SerializeField] private PauseButton _pauseButton;
    [SerializeField] private AbilityButton _bagAbilities;
    [SerializeField] private PurchaseButton _purchaseButton;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private StateTutorial _stateTutorial;
    [SerializeField] private HandMover _handMoverUI;
    [SerializeField] private CloseGameButton closeGame;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private TutorialStateMachine tutorialStateMachin;

    protected override TutorialContext CreateContext()
    {
        var context = base.CreateContext();

        _stateTutorial = GetComponent<StateTutorial>();

        if (_stateTutorial == null)
        {
            Debug.LogError($"{nameof(StateTutorial)} not found on {gameObject.name}", this);
            return context;
        }

        if (ValidateReferences() == false)
            return context;

        TutorialAbilities tutorialAbilities = new(tutorialStateMachin, context, inputHandler, _pauseButton, _text, _handMoverUI, _purchaseButton, closeGame, _bagAbilities, _hintsUI);

        context.InitScene(_key, _lock, _hints, _timer, _rotator, _container, _stateTutorial, tutorialAbilities);

        return context;
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        void Check(Object obj, string name)
        {
            if (obj == null)
            {
                Debug.LogError($"{name} is not assigned in {nameof(TutorialHandler)}", this);
                isValid = false;
            }
        }

        Check(_key, nameof(_key));
        Check(_lock, nameof(_lock));
        Check(_hints, nameof(_hints));
        Check(_timer, nameof(_timer));
        Check(_rotator, nameof(_rotator));
        Check(_container, nameof(_container));

        return isValid;
    }
}