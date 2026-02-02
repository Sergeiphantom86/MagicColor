using UnityEngine;

public class LockInteractionHandler
{
    private Lock _lock;
    private HintKey _hintKey;

    public void SetHint(HintKey hintKey) => 
        _hintKey = hintKey;

    public void TryHandle(Collider other)
    {
        if (other.TryGetComponent(out Lock @lock))
            _lock = @lock;
    }

    public void ShowBlockedFeedback()
    {
        if (_lock == null) return;

        _lock.ShakeUp();
        _hintKey.TurnOn();
    }

    public void Unblock()
    {
        _lock.Unblock();
    }
}