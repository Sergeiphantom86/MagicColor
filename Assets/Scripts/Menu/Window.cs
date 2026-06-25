using UnityEngine;
namespace Menu
{

public class Window : MonoBehaviour
{
    [SerializeField] private Background _background;

    private void OnEnable()
    {
        if (_background != null)
        {
            _background.Activate();
        }
    }

    private void OnDisable()
    {
        if (_background != null)
        {
            _background.Deactivate();
        }
    }

    public virtual void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
}