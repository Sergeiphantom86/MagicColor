using Game;
using Menu.Interaction.Abilitys;
using PuzzleResources.Counter;
using UnityEngine;
using Wallets.WalletEconomy;

namespace PuzzleResources.UI
{
    public class UISwitcher : MonoBehaviour
    {
        [SerializeField] private AnimatorPuzzle _animatorPuzzle;

        private Timer _timer;
        private BagKey _bagKey;
        private PauseButton _pauseButton;
        private AbilityButton _abilityButton;

        private void Awake()
        {
            _timer = GetComponentInChildren<Timer>();
            _bagKey = GetComponentInChildren<BagKey>();
            _pauseButton = GetComponentInChildren<PauseButton>();
            _abilityButton = GetComponentInChildren<AbilityButton>();

            if (Validate() == false)
                return;
        }

        private void OnEnable()
        {
            _animatorPuzzle.AnimationCompleted += OnTurnOffUI;
        }

        private void OnDisable()
        {
            _animatorPuzzle.AnimationCompleted -= OnTurnOffUI;
        }

        private bool Validate()
        {
            if (_timer == null)
            {
                Debug.LogError("Timer == null");
                return false;
            }

            if (_bagKey == null)
            {
                Debug.LogError("BagKey == null");
                return false;
            }

            if (_pauseButton == null)
            {
                Debug.LogError("PauseButton == null");
                return false;
            }

            if (_abilityButton == null)
            {
                Debug.LogError("AbilityButton == null");
                return false;
            }

            return true;
        }

        private void OnTurnOffUI()
        {
            _timer.gameObject.SetActive(false);
            _bagKey.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
            _abilityButton.gameObject.SetActive(false);
        }
    }
}