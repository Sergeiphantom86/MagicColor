using System.Collections;
using UnityEngine;

public class TutorialStateMachine : MonoBehaviour
{
    protected TutorialContext _context;
    private ITutorialState _currentState;

    private void Awake()
    {
        _context = new TutorialContext();
        InitializeContext();

        ChangeState(new InitializationState(this, _context));
    }

    protected virtual void InitializeContext()
    {
        _context.Mirage = GetComponentInChildren<Mirage>(true);
        _context.HandMover = GetComponentInChildren<HandMover>(true);
        _context.Visualizer = GetComponentInChildren<TouchVisualizer>(true);
    }

    private void Update()
    {
        _currentState?.Update();
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
}