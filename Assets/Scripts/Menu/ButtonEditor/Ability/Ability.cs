using UnityEngine;

namespace Menu.ButtonEditor.Ability
{
    [CreateAssetMenu(menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject
    {
        [SerializeField]
        private string _abilityName;

        [SerializeField]
        private Sprite _icon;

        public Sprite Icon => _icon;
    }
}