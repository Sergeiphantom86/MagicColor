using Menu.TutorialEditor.TutorialPuzzle;
using UnityEngine;
using PuzzleEditor.PenEditor;
namespace Menu.TutorialEditor
{

public class HintSwitcher : MonoBehaviour
{
    [SerializeField] private Blinker _backlightPause;
    [SerializeField] private Blinker _backlightPurchase;
    [SerializeField] private Blinker _backlightAbility;
    [SerializeField] private HandMover _handMover;
    [SerializeField] private Activator _activator;

    private HintCounter _hintCounter;

    private void Awake()
    {
        _hintCounter = GetComponent<HintCounter>();
    }

    private void Start()
    {
        _hintCounter.StartTimer();
    }

    private void OnEnable()
    {
        _hintCounter.Rested += DisableEveryth;
        _hintCounter.OnWorked += TurnOnBacklightPause;
        _handMover.Destroyed += ResetTimer;
        _activator.PuzzleCompleted += DisableEveryth;

        _backlightAbility.OnCompleted += Complete;
        _backlightPause.OnCompleted += TurnOnBacklightPurchase;
        _backlightPurchase.OnCompleted += TurnOnBacklightAbility;
    }

    private void OnDisable()
    {
        _hintCounter.Rested += DisableEveryth;
        _hintCounter.OnWorked -= TurnOnBacklightPause;
        _handMover.Destroyed -= ResetTimer;
        _activator.PuzzleCompleted -= DisableEveryth;

        _backlightAbility.OnCompleted -= Complete;
        _backlightPause.OnCompleted -= TurnOnBacklightPurchase;
        _backlightPurchase.OnCompleted -= TurnOnBacklightAbility;
    }

    private void TurnOnBacklightPause()
    {
        _backlightPause.Activate();
        TurnOnHand(_backlightPause.gameObject.transform);
    }

    private void TurnOnBacklightPurchase()
    {
        _backlightPurchase.Activate();
        TurnOnHand(_backlightPurchase.gameObject.transform);
    }

    private void TurnOnBacklightAbility()
    {
        _backlightAbility.Activate();
        TurnOnHand(_backlightAbility.gameObject.transform);
        ResetTimer();
    }

    private void Complete()
    {
        TurnOffHand();
    }

    private void TurnOffHand()
    {
        _handMover.Stop();
        _handMover.TurnOff();
    }

    private void TurnOnHand(Transform transform)
    {
        _handMover.TurnOn();
        _handMover.Stop();
        _handMover.SetPosition(transform.position);
        _handMover.EnableScaleAnimation();
    }

    private void DisableEveryth()
    {
        _backlightAbility.Stop();
        _backlightPause.Stop();
        _backlightPurchase.Stop();

        TurnOffHand();
    }

    private void ResetTimer()
    {
        DisableEveryth();
        _hintCounter.StartTimer();
    }
}
}