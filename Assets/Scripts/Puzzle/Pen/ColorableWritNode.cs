using UnityEngine;

public class ColorableWritNode : ColorableObject, IColorable
{
    [SerializeField] private Activator _activator;

    private void Start()
    {
        InitializeComponents();
    }

    private void OnEnable()
    {
        _activator.ColorHasChanged += SetColor;
    }

    private void OnDisable()
    {
        _activator.ColorHasChanged -= SetColor;
    }
}
