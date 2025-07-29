using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.LanguageLegacy;

public class Select : MonoBehaviour
{
    [SerializeField] private LanguageYG _taskText;
    [SerializeField] private LanguageYG _textWithoutTask;
    [SerializeField] private Button _button;

    public void ShowWithButton()
    {
        IncludeTextFromTask();
        TurnOnButton();
    }

    public void ShowWithoutButton()
    {
        IncludeTextWithoutAssignment();
        TurnOffButton();
    }

    private void IncludeTextWithoutAssignment()
    {
        _taskText.gameObject.SetActive(false);
        _textWithoutTask.gameObject.SetActive(true);
    }
    private void IncludeTextFromTask()
    {
        _taskText.gameObject.SetActive(true);
        _textWithoutTask.gameObject.SetActive(false);
    }

    private void TurnOffButton()
    {
        _button.gameObject.SetActive(false);
    }

    private void TurnOnButton()
    {
        _button.gameObject.SetActive(true);
    }
}