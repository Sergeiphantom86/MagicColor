using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonMenu : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenu;

    private Button _button;

    public PauseMenu PauseMenu => _pauseMenu;
    public Button Button => _button;

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