using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.RouletteEditor
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Button _buttonDailySpin;

        private bool _localBlock;
        private bool _isSpin;

        public bool IsSpin => _isSpin;

        public event Func<bool> GlobalInteractableCondition;

        public event Action Turned;

        private void Awake()
        {
            if (_button == null)
            {
                Debug.LogError("Button �� ��������!!!");
            }

            _button.onClick.AddListener(OnHandleClick);
        }

        public void Initialize(Func<bool> globalInteractableCondition, Action onClickAction = null)
        {
            GlobalInteractableCondition = globalInteractableCondition;
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
            _localBlock == false && (GlobalInteractableCondition?.Invoke() ?? false);

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