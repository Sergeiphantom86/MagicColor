using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClickCatcher : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AbilitySelectionManager.Instance.ClearSelection();
    }
}