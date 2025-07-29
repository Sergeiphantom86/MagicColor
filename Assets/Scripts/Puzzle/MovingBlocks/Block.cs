using System;

public class Block : ColorableObject, IDestroyable
{
    public event Action<Block> OnDestroyed;

    public void Destroy(bool destroyImmediately = false)
    {
        OnDestroyed?.Invoke(this);

        gameObject.SetActive(false);
    }
}