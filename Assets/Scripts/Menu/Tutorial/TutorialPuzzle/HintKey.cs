using System.Collections;
using UnityEngine;

public class HintKey : MonoBehaviour
{
    private float _delay;
    private bool _isTurnOn;
    private Coroutine _coroutine;
    private WaitForSeconds _delayTime;

    private void Awake()
    {
        _delay = 3;
        _delayTime = new WaitForSeconds(_delay);
    }

    public void TurnOn()
    {
        if (_isTurnOn == false)
        {
            gameObject.SetActive(true);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(WaitTurnOff());
        }
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator WaitTurnOff()
    {
        yield return _delayTime;
        TurnOff();

        _coroutine = null;
    }
}