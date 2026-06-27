using System;
using UnityEngine;

namespace Menu.ButtonEditor.Ability
{
    public class AbilitySelectionManager : MonoBehaviour
    {
        private Ability _currentAbility;
        private AbilityButton _currentButton;

        public bool HasSelection => _currentAbility != null;

        public event Action Selected;

        public void Select(AbilityButton button)
        {
            if (_currentButton != null)
            _currentButton.SetHighlight(false);

            _currentButton = button;
            _currentAbility = button.Ability;

            _currentButton.SetHighlight(true);
        }

        public void ClearSelection()
        {
            if (_currentButton == null)
            return;

            _currentButton.SetHighlight(false);
            _currentButton = null;
            _currentAbility = null;
        }

        public void Use()
        {
            Selected?.Invoke();
        }
    }
}