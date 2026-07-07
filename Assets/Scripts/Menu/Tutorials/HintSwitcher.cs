using Menu.Tutorials.TutorialPuzzle;
using PuzzleResources.PenEditor;
using UnityEngine;

namespace Menu.Tutorials
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
            _hintCounter.Rested += OnDisableEveryth;
            _hintCounter.Worked += OnTurnOnBacklightPause;
            _handMover.Destroyed += OnResetTimer;
            _activator.PuzzleCompleted += OnDisableEveryth;

            _backlightAbility.Completed += OnComplete;
            _backlightPause.Completed += OnTurnOnBacklightPurchase;
            _backlightPurchase.Completed += OnTurnOnBacklightAbility;
        }

        private void OnDisable()
        {
            _hintCounter.Rested += OnDisableEveryth;
            _hintCounter.Worked -= OnTurnOnBacklightPause;
            _handMover.Destroyed -= OnResetTimer;
            _activator.PuzzleCompleted -= OnDisableEveryth;

            _backlightAbility.Completed -= OnComplete;
            _backlightPause.Completed -= OnTurnOnBacklightPurchase;
            _backlightPurchase.Completed -= OnTurnOnBacklightAbility;
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

        private void OnTurnOnBacklightPause()
        {
            _backlightPause.Activate();
            TurnOnHand(_backlightPause.gameObject.transform);
        }

        private void OnTurnOnBacklightPurchase()
        {
            _backlightPurchase.Activate();
            TurnOnHand(_backlightPurchase.gameObject.transform);
        }

        private void OnTurnOnBacklightAbility()
        {
            _backlightAbility.Activate();
            TurnOnHand(_backlightAbility.gameObject.transform);
            OnResetTimer();
        }

        private void OnComplete()
        {
            TurnOffHand();
        }

        private void OnDisableEveryth()
        {
            _backlightAbility.Stop();
            _backlightPause.Stop();
            _backlightPurchase.Stop();

            TurnOffHand();
        }

        private void OnResetTimer()
        {
            OnDisableEveryth();
            _hintCounter.StartTimer();
        }
    }
}