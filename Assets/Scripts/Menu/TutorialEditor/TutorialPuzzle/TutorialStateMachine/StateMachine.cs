using System.Collections;
using UnityEngine;

namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        protected TutorialContext Context;
        private ITutorialState _currentState;

        private void Awake()
        {
            Context = CreateContext();
            ChangeState(new InitializationState(this, Context));
        }

        protected virtual TutorialContext CreateContext()
        {
            var context = new TutorialContext();

            context.InitBase(
                handMover: GetComponentInChildren<HandMover>(true),
                visualizer: GetComponentInChildren<TouchVisualizer>(true)
            );

            return context;
        }

        public void ChangeState(ITutorialState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            return base.StartCoroutine(routine);
        }

        public void StopCurrentCoroutine(Coroutine routine)
        {
            StopCoroutine(routine);
        }
    }
}