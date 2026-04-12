using System.Collections;
using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    private const string IsHoice = nameof(IsHoice);

    private Coroutine _coroutine;

    public void TurnOff()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(WaitCompletion());
    }

    private IEnumerator WaitCompletion()
    {
        yield return new WaitForSeconds(0.2f);

    }
}