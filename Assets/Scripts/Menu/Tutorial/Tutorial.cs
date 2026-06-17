using UnityEngine;

[RequireComponent(typeof(EngineTutorialMenu))]
public class Tutorial : MonoBehaviour
{
    private const string IsSwipe = nameof(IsSwipe);
    private const string IsClick = nameof(IsClick);

    private bool _isFinished;
    private EngineTutorialMenu _engineTutorialMenu;
    private IProgressSaver _progressSaver;

    public bool IsSwipeAllowed { get; private set; }

    public bool IsClickAllowed { get; private set; }

    public bool IsTutorialActive => gameObject.activeSelf;

    private void Awake()
    {
        _engineTutorialMenu = GetComponent<EngineTutorialMenu>();
        _progressSaver = new ProgressSaver();

        _isFinished = _progressSaver.Saves.IsMenuTutorial;

        if (_isFinished)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetPositionButton(Vector3 position)
    {
        _engineTutorialMenu.SetPosition(position);
    }

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        IsSwipeAllowed = true;
        IsClickAllowed = false;

        _engineTutorialMenu.StartAnimationMovements();
    }

    public void CompleteSwapStep()
    {
        _engineTutorialMenu.StartAnimationClicks();

        IsSwipeAllowed = false;
        IsClickAllowed = true;
    }

    public void CompleteClickStep()
    {
        Finish();

        _engineTutorialMenu.StopAnimation();
    }

    private void Finish()
    {
        gameObject.SetActive(false);

        IsSwipeAllowed = false;
        IsClickAllowed = false;

        _progressSaver.DisableTutorialMenu();
    }
}