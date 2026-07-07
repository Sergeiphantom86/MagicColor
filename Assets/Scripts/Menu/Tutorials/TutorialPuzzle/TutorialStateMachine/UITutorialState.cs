using System.Collections;
using DG.Tweening;
using Menu.Tutorials.TutorialUI;
using PuzzleResources.Counter;
using PuzzleResources.Stars;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle.TutorialStateMachine
{
    public class UITutorialState : TutorialStater
    {
        private readonly float _scaleMultiplier;
        private readonly float _duration;
        private readonly StateMachine _tutorialStateMachine;
        private readonly TutorialContext _context;

        private int _currentCountStars;
        private StarsCounter _starsCounter;
        private TimerFringe _timerFringe;
        private Vector3 _startScale;
        private Timer _timer;
        private Coroutine _coroutine;

        public UITutorialState(StateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context)
        {
            _scaleMultiplier = 1.2f;
            _duration = 0.3f;

            _tutorialStateMachine = stateMachine;
            _context = context;
        }

        public override void Enter()
        {
            if (TryInitializeComponents() == false)
                return;

            _currentCountStars = _starsCounter.MaxStars;

            _timerFringe.SetActive(true);

            _startScale = _timer.transform.localScale;

            _coroutine = _tutorialStateMachine.StartCoroutine(WaitForOneStarLost());
            _tutorialStateMachine.StartCoroutine(TutorialFlow());
        }

        public override void Exit()
        {
            _timerFringe.SetActive(false);
        }

        private IEnumerator TutorialFlow()
        {
            yield return _context.WaitStarTurnOff;

            _currentCountStars = GetCountStars();

            yield return _context.WaitUIDisabled;

            _tutorialStateMachine.ChangeState(new BlockTutorialState(_tutorialStateMachine, _context, _starsCounter));
        }

        private IEnumerator WaitForOneStarLost()
        {
            while (_currentCountStars == _starsCounter.MaxStars)
            {
                _timer.transform.DOScale(_startScale * _scaleMultiplier, _duration);

                yield return _context.WaitForSeconds;

                _timer.transform.DOScale(_startScale, _duration);

                yield return _context.WaitForSeconds;
            }
        }

        private int GetCountStars()
        {
            return _starsCounter.DisableOneStar();
        }

        private bool TryInitializeComponents()
        {
            if (_context.Timer == null)
                return Fail("Context.Timer == null", _context.Timer);

            CacheComponents();

            if (_timerFringe == null)
                return Fail("TimerFringe not found on Timer", _timerFringe);

            if (_starsCounter == null)
                return Fail("StarsCounter == null in TimerFringe", _starsCounter);

            return true;
        }

        private void CacheComponents()
        {
            _timer = _context.Timer;

            _timerFringe = _timer.GetComponentInChildren<TimerFringe>(true);

            _timerFringe.Button.onClick.AddListener(OnFinishClick);

            _starsCounter = _timerFringe.StarsCounter;
        }

        private bool Fail(string message, Object context)
        {
            Debug.LogError(message, context);
            return false;
        }

        private void OnFinishClick()
        {
            if (_coroutine != null)
            {
                _tutorialStateMachine.StopCoroutine(_coroutine);
            }

            _timer.transform.DOScale(_startScale, _duration);

            _tutorialStateMachine.ChangeState(new BlockTutorialState(_tutorialStateMachine, _context, _starsCounter));
        }
    }
}