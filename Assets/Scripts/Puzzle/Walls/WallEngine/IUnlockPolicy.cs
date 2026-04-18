public interface IUnlockPolicy
{
    bool TryUnlock();

    public void Use();
}