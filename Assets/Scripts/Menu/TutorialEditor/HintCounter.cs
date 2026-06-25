using Game.SaveEditor;
using PuzzleEditor;
using System;
using System.Collections;
using UnityEngine;
namespace Menu.TutorialEditor
{

public class HintCounter : MonoBehaviour
{
    [SerializeField] private BlocksContainer _container;

    private Coroutine _hintCoroutine;
    private float _hintDelay;
    private IProgressSaver _progressSaver;

    public event Action OnWorked;

    public event Action Rested;

    private void Awake()
    {
        _hintDelay = 60;
        _progressSaver = new ProgressSaver();
    }

    private void OnEnable()
    {
        _container.OneDestroyed += StartTimer;
        _container.EverythDestroyed += StopTimer;
    }

    private void OnDisable()
    {
        _container.OneDestroyed -= StartTimer;
        _container.EverythDestroyed += StopTimer;
    }

    public void StartTimer()
    {
        Rested?.Invoke();

        if (_progressSaver.Saves.IsUnlockAbilities && _progressSaver.Saves.CurrentCoin >= 3000)
        {
            ResetTimer();
        }
    }

    public void ResetTimer()
    {
        StopTimer();

        _hintCoroutine = StartCoroutine(ShowHintAfterDelay());
    }

    private IEnumerator ShowHintAfterDelay()
    {
        yield return new WaitForSeconds(_hintDelay);

        OnWorked?.Invoke();
    }

    private void StopTimer()
    {
        if (_hintCoroutine != null)
        {
            StopCoroutine(_hintCoroutine);
            _hintCoroutine = null;
        }
    }
}
}