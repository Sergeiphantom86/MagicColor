using UnityEngine;

public abstract class InstallerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    protected T Item;
    protected bool IsPlaced;

    private IProgressSaver _progressSaver;

    protected virtual void Awake()
    {
        //_progressSaver = new ProgressSaver();

        //if (_progressSaver.Saves.IsUnlockKey == false)
        //    gameObject.SetActive(false);
    }

    public void Initialize(T item)
    {
        Item = item;
    }

    //protected bool CanPlace()
    //{
    //    //return _progressSaver.Saves.IsUnlockKey &&
    //    //       Item != null &&
    //    //       IsPlaced == false &&
    //    //       CanPlaceInternal();
    //}

    protected virtual bool CanPlaceInternal() => true;
}