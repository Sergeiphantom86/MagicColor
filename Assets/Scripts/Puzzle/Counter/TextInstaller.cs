using TMPro;
using UnityEngine;

public class TextInstaller : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    [SerializeField] private TextMeshProUGUI _completionTime;
    [SerializeField] private TextMeshProUGUI _countStars;

    private TMP_Text _text;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _progressSaver = new ProgressSaver();
    }

    private void Start()
    {
        _text.text = $"{_completionTime.text} {_timer.TimerText.text}, {_countStars.text} {_progressSaver.Saves.CountStars}";
    }
}