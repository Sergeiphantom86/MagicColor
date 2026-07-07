using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu.Interaction.Abilitys
{
    public class BackgroundClickCatcher : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AbilitySelectionManager _abilitySelectionManager;

        public void OnPointerClick(PointerEventData eventData)
        {
            _abilitySelectionManager.ClearSelection();
        }
    }
}