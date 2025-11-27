using System;
using UnityEngine;

public class StateTutorial : MonoBehaviour
{
    private Key _key;
    private Lock _lock;
    private HandMover _handMover;
    private TouchVisualizer  _visualizer;
    private bool _isClick;

    public event Action OnCompleted;

    private void OnDisable()
    {
        if (_key == null) return;
        if (_lock == null) return;

        _key.OnShift -= MovePointer;
        _key.OnSelected -= MovePointer—lick;
        _lock.OnUnblocking -= Complete;
    }

    public void Initialization(HandMover handMover, TouchVisualizer touchVisualizer, Key key, Lock @lock)
    {
        _key = key;
        _lock = @lock;
        _handMover = handMover;
        _visualizer = touchVisualizer;
        
        SubscribeEvents();

        Begin();
    }

    private void SubscribeEvents()
    {
        _key.OnShift += MovePointer;
        _key.OnSelected += MovePointer—lick;
        _lock.OnUnblocking += Complete;
    }

    private void Begin()
    {
        _handMover.EnableLoopingAnimationZ();
    }

    private void MovePointer—lick()
    {
        SetPositionsEquipment(_lock.transform.position);
    }

    private void MovePointer()
    {
        if (_isClick == false)
        {
            _isClick = true;

            SetPositionsEquipment(_key.transform.position);

            _visualizer.gameObject.SetActive(true);

            _handMover.Stop();
            _handMover.EnableScaleAnimation();
        }
    }

    private void Complete()
    {
        _visualizer.gameObject.SetActive(false);
        _handMover.gameObject.SetActive(false);

        OnCompleted?.Invoke();
    }

    private void SetPositionsEquipment(Vector3 position)
    {
        _handMover.SetPosition(position);
        _visualizer.SetPosition(position);
    }
}