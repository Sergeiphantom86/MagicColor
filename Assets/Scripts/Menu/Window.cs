using UnityEngine;

public class Window : MonoBehaviour
{
    public virtual void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            OnShow();
        }
        else
        {
            OnHide();
        }
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}