using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu.ButtonEditor.Ability
{
    public class BackgroundClickCatcher : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            AbilitySelectionManager.Instance.ClearSelection();
        }
    }
}