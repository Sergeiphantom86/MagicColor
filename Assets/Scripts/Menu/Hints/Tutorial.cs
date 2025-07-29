using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Tutorial : MonoBehaviour
{
    private const string IsSwipe = nameof(IsSwipe);
    private const string IsClick = nameof(IsClick);

    private static bool s_isFinished;

    private Animator _animator;
    private ParticleSystem _particleSystem;

    public bool IsSwipeAllowed { get; private set; }
    public bool IsClickAllowed { get; private set; }
    public bool IsTutorialActive => gameObject.activeSelf;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();


        if (_animator == null)
        {
            Debug.LogError("Animator не назначен!");
            return;
        }

        if (_particleSystem == null)
        {
            Debug.LogError("Particle System не назначается и не обнаруживается у детей!");
            return;
        }

        if (s_isFinished)
        {
            gameObject.SetActive(false);
        }

        StartTutorial();
    }

    public void StartTutorial()
    {
        _particleSystem?.Play();

        IsSwipeAllowed = true;
        IsClickAllowed = false;
    }

    public void ResetTutorial()
    {
        _animator.SetBool(IsSwipe, false);
        gameObject.SetActive(true);
        s_isFinished = false;
    }

    public void CompleteSwapStep()
    {
        _animator.SetBool(IsSwipe, true);
        _particleSystem?.Stop();

        IsSwipeAllowed = false;
        IsClickAllowed = true;
    }

    public void CompleteClickStep()
    {
        FinishTutorial();
    }

    private void FinishTutorial()
    {
        _particleSystem.Stop();
        gameObject.SetActive(false);

        IsSwipeAllowed = false;
        IsClickAllowed = false;
        s_isFinished = true;
    }
}