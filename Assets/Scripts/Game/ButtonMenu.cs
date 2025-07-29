using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonMenu : MonoBehaviour
{
    [SerializeField] private PauseManager _pauseManager;

    private Button _button;

    public PauseManager PauseManager => _pauseManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(PressButton);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(PressButton);
    }

    public virtual void PressButton() { }
}