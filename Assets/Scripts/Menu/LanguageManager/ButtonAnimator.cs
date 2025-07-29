using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ButtonAnimator : MonoBehaviour
{
    private const string IsHoice = nameof(IsHoice);

    private Animator _animator;
    private Coroutine _coroutine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator не назначен!");
        }

        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        _animator.SetBool(IsHoice, true);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(WaitCompletion());
    }

    private IEnumerator WaitCompletion()
    {
        yield return new WaitForSeconds(0.2f);

        gameObject.SetActive(false);
    }
}