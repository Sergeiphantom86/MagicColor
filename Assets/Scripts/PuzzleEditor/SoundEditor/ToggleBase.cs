using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.SoundEditor
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleBase : MonoBehaviour
    {
        private Toggle _toggle;

        public event Action<bool> OnDisabling;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();

            if (_toggle == null)
            {
                Debug.LogError("Toggle �� ��������!");
                return;
            }
        }

        private void OnEnable()
        {
            if (_toggle == null)
                return;

            _toggle.onValueChanged.AddListener(TurnOff);
        }

        private void OnDisable()
        {
            if (_toggle == null)
                return;

            _toggle.onValueChanged.RemoveListener(TurnOff);
        }

        public void TurnOff(bool isOn)
        {
            OnDisabling?.Invoke(isOn);
        }

        public void TurnOn(bool isOn)
        {
            _toggle.isOn = isOn;
        }
    }
}