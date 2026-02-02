using UnityEngine;

public class LockFeedbackService : MonoBehaviour, ILockFeedbackService
{
    private Lock _lock;
    private HintKey _hint;

    public void InitializComponents(Lock @lock, HintKey hint)
    {
        SetLock(@lock);
        SetKey(hint);
    }

    private void SetLock(Lock @lock)
    {
        if (@lock == null)
        {
            Debug.LogError("Lock == null");
            return;
        }
        
        _lock = @lock;
    }

    private void SetKey(HintKey hint)
    {
        if (hint == null)
        {
            Debug.LogError("HintKey == null");
            return;
        }

        _hint = hint;
    }

    public void Play()
    {
        if (_lock != null)
        {
            _lock.ShakeUp();
        }

        if (_hint != null)
        {
            _hint.TurnOn();
        }
    }
}