using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private string _timeFormat = "mm':'ss";
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private LeaderboardStars _leaderboardStars;

    private float _value;
    private TimeSpan _span;

    public bool IsRunning { get; private set; }

    public event Action<string, float> OnRecord;

    private void Update()
    {
        if (PauseManager.IsPaused) return;

        if (IsRunning)
        {
            _value += Time.unscaledDeltaTime;

            _span = TimeSpan.FromSeconds(_value);
            _timerText.text = _span.ToString(_timeFormat);
        }
    }

    private void OnEnable()
    {
        _blocksContainer.StoppingTimer += StopAndSave;
    }

    private void OnDisable()
    {
        _blocksContainer.StoppingTimer -= StopAndSave;
    }

    public void StartTimer()
    {
        if (IsRunning) return;
        IsRunning = true;
        _value = 0f;
    }

    public void StopAndSave(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Sprite name is null or empty");
            return;
        }

        Stop();

        if (_leaderboardStars != null)
        {
            _leaderboardStars.SavePlayerTime(_value);
        }
        else
        {
            Debug.LogError("LeaderboardStars not found!");
        }

        if (PuzzleDataTransfer.PuzzleResultTime > _value || PuzzleDataTransfer.PuzzleResultTime == 0 )
        {
            PuzzleDataTransfer.PuzzleResultTime = _value;
            OnRecord?.Invoke(GetSpriteKey(name), _value);
        }
        else
        {
            PuzzleDataTransfer.PuzzleResultTime = _value;
        }
    }

    public void Stop()
    {
        if (IsRunning == false) return;
        IsRunning = false;
    }

    private string GetSpriteKey(string puzzleId)
    {
        return $"{puzzleId}";
    }
}