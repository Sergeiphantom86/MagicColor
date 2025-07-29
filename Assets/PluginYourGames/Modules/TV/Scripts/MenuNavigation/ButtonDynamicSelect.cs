using UnityEngine;
using UnityEngine.UI;

namespace YG.MenuNav
{
    public class ButtonDynamicSelect : MonoBehaviour
    {
        public Button buttonCash;

        public virtual void Start()
        {
            if (buttonCash == false)
                buttonCash.GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (MenuNavigation.Instance)
                MenuNavigation.Instance.AddButtonList(buttonCash);
        }

        public virtual void OnDisable()
        {
            if (MenuNavigation.Instance)
                MenuNavigation.Instance.RemoveButtonList(buttonCash);
        }
    }
}