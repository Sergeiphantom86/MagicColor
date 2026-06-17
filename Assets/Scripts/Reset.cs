using UnityEngine;
using UnityEngine.UI;

public class Reset : MonoBehaviour
{
    private Button _button;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _progressSaver = new ProgressSaver();
    }

    private void Start()
    {
        _button.onClick.AddListener(TurnOn);
    }

    private void TurnOn()
    {
        _progressSaver.SetDefaultValues();
        _progressSaver.SaveProgress();
        SceneLoader.Instance.LoadSceneAsyncWithSplash("Menu");
    }
}