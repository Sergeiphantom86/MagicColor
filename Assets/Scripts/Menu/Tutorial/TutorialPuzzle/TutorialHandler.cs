using UnityEngine;

public class TutorialHandler : TutorialStateMachine
{
    [SerializeField] private Hints _hints;
    [SerializeField] private Key _key;
    [SerializeField] private Lock _lock;
    [SerializeField] private Rotator _rotator;
    [SerializeField] private BlockSpawner _container;
    [SerializeField] private MenuLoader _menuLoader;

    private StateTutorial _stateTutorial;

    protected override void InitializeContext()
    {
        base.InitializeContext();

        _stateTutorial = GetComponent<StateTutorial>();

        _context.Hints = _hints;
        _context.Key = _key;
        _context.Lock = _lock;
        _context.Rotator = _rotator;
        _context.Container = _container;
        _context.MenuLoader = _menuLoader;
        _context.StateTutorial = _stateTutorial;
    }
}