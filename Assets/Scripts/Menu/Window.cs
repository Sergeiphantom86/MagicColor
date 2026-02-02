using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] private GameObject _background;

    private void OnEnable()
    {
        if (_background != null)
        {
            _background.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (_background != null)
        {
            _background.SetActive(false);
        }
    }

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