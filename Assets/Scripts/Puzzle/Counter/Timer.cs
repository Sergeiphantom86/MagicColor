using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private string _timeFormat = "mm':'ss";
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private StarCounter _starCounter;

    private float _value;
    private bool _isRunning;
    private TimeSpan _span;

    private void Update()
    {
        if (_isRunning)
        {
            _value += Time.unscaledDeltaTime;

            _span = TimeSpan.FromSeconds(_value);
            _timerText.text = _span.ToString(_timeFormat);
        }
    }

    private void OnEnable()
    {
        _blocksContainer.BlockDestroyed += StopAndSave;
    }

    private void OnDisable()
    {
        _blocksContainer.BlockDestroyed -= StopAndSave;
    }

    public void StartTimer()
    {
        if (_isRunning) return;
        _isRunning = true;
        _value = 0f;
    }

    public void StopAndSave()
    {
        if (_starCounter == null)
        {
            Debug.LogError("StarCounter not found!");
            return;
        }


        Stop();

        _starCounter.SavePlayerTime(_value);
    }

    public void Stop()
    {
        if (_isRunning == false) return;
        _isRunning = false;
    }
}