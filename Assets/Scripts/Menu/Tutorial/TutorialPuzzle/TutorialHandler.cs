using System.Collections;
using UnityEngine;
using YG;

[RequireComponent(typeof(StateTutorial))]
public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private Hints _hints;
    [SerializeField] private Key _key;
    [SerializeField] private Lock _lock;
    [SerializeField] private Rotator _rotator;
    [SerializeField] private BlockSpawner _container;
    [SerializeField] private MenuLoader _menuLoader;

    private int _index;
    private float _delay;
    private bool _isAnimationChange;
    private Block _block;
    private Mirage _mirage;
    private HandMover _handMover;
    private TouchVisualizer _visualizer;
    private TouchDragInput _touchDragInput;
    private StateTutorial _stateTutorial;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _index = 4;
        _delay = 1;
        _mirage = GetComponentInChildren<Mirage>(true);
        _handMover = GetComponentInChildren<HandMover>(true);
        _visualizer = GetComponentInChildren<TouchVisualizer>(true);
        _stateTutorial = GetComponent<StateTutorial>();
        _waitForSeconds = new WaitForSeconds(_delay);

        if (_mirage == null)
        {
            Debug.LogError("Mirage == null");
            return;
        }


        if (_handMover == null)
        {
            Debug.LogError("HandMover == null");
            return;
        }


        if (_visualizer == null)
        {
            Debug.LogError("TouchVisualizer == null");
            return;
        }


        if (_stateTutorial == null)
        {
            Debug.LogError("StateTutorial == null");
            return;
        }

        _hints.gameObject.SetActive(false);
        _visualizer.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _stateTutorial.OnInstalled += SetPosition;
        _rotator.OnRotated += Initialization;
        _mirage.OnMovement += ContinueDriving;
        _stateTutorial.OnCompleted += Load;
        _mirage.OnCompleted += Load;
    }

    private void OnDisable()
    {
        if (_touchDragInput == null) return;
        if (_stateTutorial == null) return;
        if (_rotator == null) return;
        if (_mirage == null) return;

        _stateTutorial.OnInstalled -= SetPosition;
        _touchDragInput.OnTouchClick -= Click;
        _rotator.OnRotated -= Initialization;
        _mirage.OnMovement -= ContinueDriving;
        _stateTutorial.OnCompleted -= Load;
        _mirage.OnCompleted -= Load;
    }

    private void Initialization()
    {
        if (YG2.saves.IsTutorial)
        {
            SetPosition(_key.transform.position);

            _stateTutorial.Initialization(_handMover, _visualizer, _key, _lock);

            return;
        }

        if (_block == null)
        {
            SetBlock(_index);
        }
        
        _touchDragInput = _block.GetComponent<TouchDragInput>();

        SetPosition(_block.transform.position);

        _visualizer.gameObject.SetActive(true);

        _handMover.EnableScaleAnimation();

        DisableUnnecessaryComponents();

        _touchDragInput.OnTouchClick += Click;
    }

    private void SetBlock(int index)
    {
        if (_container.SpawnedBlocks == null || index < 0 || index >= _container.SpawnedBlocks.Count)
        {
            Debug.LogError($"Invalid index: {index}, list count: {_container.SpawnedBlocks?.Count}");
        }

        _block = _container.SpawnedBlocks[index];
    }

    private void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    private void Click(Vector2 position)
    {
        if (_isAnimationChange == false)
        {
            _isAnimationChange = true;

            _handMover.Stop();

            TurnOffComponets();
            _block.gameObject.SetActive(false);
            MoveZ();
        }
    }

    private void TurnOffComponets()
    {
        _visualizer.gameObject.SetActive(false);
        _mirage.gameObject.SetActive(true);
    }

    private void ContinueDriving()
    {
        StartCoroutine(WaitMove());
    }

    private void MoveZ()
    {
        _handMover.EnableMoveAnimationZ();
        _mirage.EnableMoveAnimationZ();
    }

    private void DisableUnnecessaryComponents()
    {
        _lock.gameObject.SetActive(false);
        _key.gameObject.SetActive(false);
    }

    private void Load()
    {
        StartCoroutine(WaitCompletion());
    }

    private IEnumerator WaitCompletion()
    {
        yield return _waitForSeconds;

        _hints.TurnOn(false);

        yield return _waitForSeconds;

        TurnOffVisualDisplay();

        yield return _waitForSeconds;

        TurnOffHints();

        _menuLoader.SaveCurrency();
    }

    private IEnumerator WaitMove()
    {
        _hints.TurnOn(true);

        yield return IncreaseWaitTime();

        TurnOffHints();

        MoveX();
    }

    private IEnumerator IncreaseWaitTime()
    {
        yield return _waitForSeconds;
        yield return _waitForSeconds;
        yield return _waitForSeconds;
    }

    private void TurnOffVisualDisplay()
    {
        _mirage.gameObject.SetActive(false);
        _handMover.gameObject.SetActive(false);
    }

    private void MoveX()
    {
        _handMover.EnableMoveAnimationX();
        _mirage.EnableMoveAnimationX();
    }

    private void TurnOffHints()
    {
        _hints.gameObject.SetActive(false);
    }
}