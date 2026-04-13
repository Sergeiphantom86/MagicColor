using System;
using System.Collections;
using UnityEngine;

public class HintCounter : MonoBehaviour
{
    private Coroutine _hintCoroutine;
    private float _hintDelay;

    public event Action OnWorked;

    private void Awake()
    {
        _hintDelay = 3;
    }

    private void Start()
    {
        ResetTimer();
    }

    public void ResetTimer()
    {
        if (_hintCoroutine != null)
            StopCoroutine(_hintCoroutine);
        _hintCoroutine = StartCoroutine(ShowHintAfterDelay());
    }

    public void ShowHint()
    {
        if (_hintCoroutine != null)
            StopCoroutine(_hintCoroutine);
    }

    public void HideHintAndRestart()
    {
        ResetTimer();
    }

    private IEnumerator ShowHintAfterDelay()
    {
        yield return new WaitForSeconds(_hintDelay);

        OnWorked?.Invoke();
    }
}