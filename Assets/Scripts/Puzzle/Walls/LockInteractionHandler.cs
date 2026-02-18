using UnityEngine;

public class LockInteractionHandler
{
    private Lock _lock;
    private HintKey _hintKey;
    private bool _initialized;

    public void SetHint(HintKey hintKey) => 
        _hintKey = hintKey;

    public void Set(Collider other)
    {
        if (other.TryGetComponent(out Lock @lock))
        {
            if (_initialized == false)
            {
                _initialized = true;
                _lock = @lock;
            }
        }
    }

    public void ShowBlockedFeedback()
    {
        if (_lock == null) return;

        _lock.ShakeUp();
        _hintKey.TurnOn();
    }

    public void Unblock()
    {
        if (_lock != null)
        {
            _lock.Unblock();
        }
    }
}