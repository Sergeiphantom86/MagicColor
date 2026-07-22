using Menu.Tutorials.TutorialPuzzle.TutorialStateMachine;
using PuzzleResources;
using PuzzleResources.Counter;
using PuzzleResources.LockMechanics;
using PuzzleResources.Spawners;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle
{
    public class TutorialHandler : StateMachine
    {
        [SerializeField] private Key _key;
        [SerializeField] private Lock _lock;
        [SerializeField] private Hints _hints;
        [SerializeField] private Timer _timer;
        [SerializeField] private Rotator _rotator;
        [SerializeField] private BlockSpawner _container;
        [SerializeField] private StateTutorial _stateTutorial;

        private TutorialAbilitiesHandler _tutorialAbilitiesHandler;

        private void Awake()
        {
            _tutorialAbilitiesHandler = GetComponent<TutorialAbilitiesHandler>();
        }

        protected override TutorialContext CreateContext()
        {
            var context = base.CreateContext();

            _stateTutorial = GetComponent<StateTutorial>();

            if (_stateTutorial == null)
            {
                Debug.LogError($"{nameof(StateTutorial)} not found on {gameObject.name}", this);
                return context;
            }

            if (ValidateReferences() == false)
                return context;

            _tutorialAbilitiesHandler.SetContext(context);

            context.InitScene(
                _key,
                _lock,
                _hints,
                _timer,
                _rotator,
                _container,
                _stateTutorial,
                _tutorialAbilitiesHandler.Abilities);

            return context;
        }

        private bool ValidateReferences()
        {
            bool isValid = true;

            void Check(Object obj, string name)
            {
                if (obj == null)
                {
                    Debug.LogError($"{name} is not assigned in {nameof(TutorialHandler)}", this);
                    isValid = false;
                }
            }

            Check(_key, nameof(_key));
            Check(_lock, nameof(_lock));
            Check(_hints, nameof(_hints));
            Check(_timer, nameof(_timer));
            Check(_rotator, nameof(_rotator));
            Check(_container, nameof(_container));

            return isValid;
        }
    }
}