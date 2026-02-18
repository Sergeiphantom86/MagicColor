using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Icon : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        _progressSaver = new ProgressSaver();  

        SetRank(SetLexel().ToString().NullIfEmpty());
    }

    public void SetRank(string rank)
    {
        if (_textMeshProUGUI == null) return;
       
        _textMeshProUGUI.text = rank;
    }

    private int SetLexel()
    {
        int value = 1;

        value += _progressSaver.Saves.QuestIndex;

        return value;
    }
}