using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleResources.MinigamesRoulette
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Button _buttonDailySpin;

        private bool _localBlock;
        private bool _isSpin;

        public event Func<bool> GlobalInteractabled;

        public event Action Turned;

        public bool IsSpin => _isSpin;

        private void Awake()
        {
            if (_button == null)
            {
                Debug.LogError($"Button is null on {gameObject.name}!");
            }

            _button.onClick.AddListener(OnHandleClick);
        }

        public void Initialize(Func<bool> globalInteractabled, Action onClickAction = null)
        {
            GlobalInteractabled = globalInteractabled;
            Turned = onClickAction;

            UpdateState();
        }

        public void SetLocalBlock(bool block)
        {
            _localBlock = block;

            UpdateState();
        }

        public void UpdateState()
        {
            bool isInteractable =
            _localBlock == false && (GlobalInteractabled?.Invoke() ?? false);

            _button.interactable = isInteractable;
        }

        private void OnHandleClick()
        {
            if (_button.interactable)
            {
                Turned?.Invoke();
                _isSpin = true;
            }
        }
    }
}