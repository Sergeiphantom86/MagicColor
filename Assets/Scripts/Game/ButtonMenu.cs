using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonMenu : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenu;

    private Button _button;
    private IProgressSaver _progressSaver;

    public PauseMenu PauseMenu => _pauseMenu;
    public Button Button => _button;

    public IProgressSaver ProgressSaver => _progressSaver;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _progressSaver = new ProgressSaver();
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